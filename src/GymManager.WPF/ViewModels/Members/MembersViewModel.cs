using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymManager.Domain.Entities;
using GymManager.Infrastructure.Persistence;
using GymManager.WPF.Views.Members;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GymManager.WPF.ViewModels.Members;

/// <summary>
/// ViewModel para la lista de miembros (CRUD completo)
/// </summary>
public partial class MembersViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private ObservableCollection<Member> _members = new();

    [ObservableProperty]
    private Member? _selectedMember;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = "Listo";

    [ObservableProperty]
    private int _totalCount;

    public MembersViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Carga la lista de miembros
    /// </summary>
    [RelayCommand]
    public async Task LoadMembersAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Cargando miembros...";

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            var query = context.Members
                .Where(m => m.IsActive && m.DeletedAt == null);

            // Aplicar filtro de busqueda
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var search = SearchText.ToLower();
                query = query.Where(m =>
                    m.FirstName.ToLower().Contains(search) ||
                    m.LastName.ToLower().Contains(search) ||
                    m.MemberCode.ToLower().Contains(search) ||
                    (m.Email != null && m.Email.ToLower().Contains(search)) ||
                    (m.Phone != null && m.Phone.Contains(search)));
            }

            var membersList = await query
                .OrderBy(m => m.FirstName)
                .ThenBy(m => m.LastName)
                .ToListAsync();

            Members.Clear();
            foreach (var member in membersList)
            {
                Members.Add(member);
            }

            TotalCount = Members.Count;
            StatusMessage = $"{TotalCount} miembros encontrados";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            MessageBox.Show($"Error al cargar miembros: {ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Busca miembros
    /// </summary>
    [RelayCommand]
    private async Task SearchAsync()
    {
        await LoadMembersAsync();
    }

    /// <summary>
    /// Abre ventana modal para agregar nuevo miembro
    /// </summary>
    [RelayCommand]
    private async Task AddMemberAsync()
    {
        try
        {
            var window = new MemberFormWindow(_serviceProvider)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            await window.InitializeNewAsync();

            var result = window.ShowDialog();

            if (result == true && window.IsSaved)
            {
                // Refrescar lista
                await LoadMembersAsync();
                StatusMessage = "Miembro agregado correctamente";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al abrir formulario: {ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Abre ventana modal para editar miembro seleccionado
    /// </summary>
    [RelayCommand]
    private async Task EditMemberAsync()
    {
        if (SelectedMember == null)
        {
            MessageBox.Show("Seleccione un miembro para editar", "Aviso",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        try
        {
            var window = new MemberFormWindow(_serviceProvider)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            await window.InitializeEditAsync(SelectedMember.MemberId);

            var result = window.ShowDialog();

            if (result == true && window.IsSaved)
            {
                // Refrescar lista
                await LoadMembersAsync();
                StatusMessage = "Miembro actualizado correctamente";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al abrir formulario: {ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Elimina el miembro seleccionado (soft delete)
    /// </summary>
    [RelayCommand]
    private async Task DeleteMemberAsync()
    {
        if (SelectedMember == null)
        {
            MessageBox.Show("Seleccione un miembro para eliminar", "Aviso",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var result = MessageBox.Show(
            $"Esta seguro de eliminar a {SelectedMember.FirstName} {SelectedMember.LastName}?\n\nEsta accion no se puede deshacer.",
            "Confirmar Eliminacion",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            IsLoading = true;

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            var member = await context.Members.FindAsync(SelectedMember.MemberId);
            if (member != null)
            {
                // Soft delete
                member.IsActive = false;
                member.DeletedAt = DateTime.UtcNow;
                await context.SaveChangesAsync();

                Members.Remove(SelectedMember);
                TotalCount = Members.Count;
                StatusMessage = "Miembro eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            MessageBox.Show($"Error al eliminar: {ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Refresca la lista de miembros
    /// </summary>
    [RelayCommand]
    private async Task RefreshAsync()
    {
        SearchText = string.Empty;
        await LoadMembersAsync();
    }
}