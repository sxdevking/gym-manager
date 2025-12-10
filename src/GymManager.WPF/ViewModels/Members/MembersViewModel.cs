using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymManager.Application.Common.Interfaces;
using GymManager.Domain.Entities;
using GymManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GymManager.WPF.ViewModels.Members;

/// <summary>
/// ViewModel para la lista de miembros
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

    // Evento para solicitar apertura de formulario
    public event Action<Member?>? OnEditMemberRequested;

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

            var membersList = await context.Members
                .Where(m => m.IsActive && m.DeletedAt == null)
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
    /// Busca miembros por nombre o código
    /// </summary>
    [RelayCommand]
    public async Task SearchMembersAsync()
    {
        try
        {
            IsLoading = true;

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            var query = context.Members
                .Where(m => m.IsActive && m.DeletedAt == null);

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchLower = SearchText.ToLower();
                query = query.Where(m =>
                    m.FirstName.ToLower().Contains(searchLower) ||
                    m.LastName.ToLower().Contains(searchLower) ||
                    m.MemberCode.ToLower().Contains(searchLower) ||
                    (m.Email != null && m.Email.ToLower().Contains(searchLower)) ||
                    (m.Phone != null && m.Phone.Contains(searchLower)));
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
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Abre el formulario para nuevo miembro
    /// </summary>
    [RelayCommand]
    private void AddMember()
    {
        OnEditMemberRequested?.Invoke(null);
    }

    /// <summary>
    /// Abre el formulario para editar miembro
    /// </summary>
    [RelayCommand]
    private void EditMember()
    {
        if (SelectedMember != null)
        {
            OnEditMemberRequested?.Invoke(SelectedMember);
        }
    }

    /// <summary>
    /// Elimina el miembro seleccionado (soft delete)
    /// </summary>
    [RelayCommand]
    private async Task DeleteMemberAsync()
    {
        if (SelectedMember == null) return;

        var result = MessageBox.Show(
            $"¿Está seguro de eliminar a {SelectedMember.FirstName} {SelectedMember.LastName}?",
            "Confirmar Eliminación",
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
    /// Refresca la lista después de guardar
    /// </summary>
    public async Task RefreshAfterSaveAsync()
    {
        await LoadMembersAsync();
    }
}