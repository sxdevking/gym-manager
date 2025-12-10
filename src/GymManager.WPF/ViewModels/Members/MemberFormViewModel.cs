using AutoMapper.Execution;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymManager.Domain.Entities;
using GymManager.Domain.Enums;
using GymManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Windows;

namespace GymManager.WPF.ViewModels.Members;

/// <summary>
/// ViewModel para el formulario de miembro (Agregar/Editar)
/// </summary>
public partial class MemberFormViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    private Guid? _memberId;

    [ObservableProperty]
    private string _title = "Nuevo Miembro";

    [ObservableProperty]
    private string _memberCode = string.Empty;

    [ObservableProperty]
    private string _firstName = string.Empty;

    [ObservableProperty]
    private string _lastName = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _phone = string.Empty;

    [ObservableProperty]
    private DateTime? _birthDate;

    [ObservableProperty]
    private Gender? _selectedGender;

    [ObservableProperty]
    private string _address = string.Empty;

    [ObservableProperty]
    private string _emergencyContact = string.Empty;

    [ObservableProperty]
    private string _emergencyPhone = string.Empty;

    [ObservableProperty]
    private string _notes = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    // Lista de géneros para el ComboBox
    public IEnumerable<Gender> Genders => Enum.GetValues<Gender>();

    // Evento cuando se guarda exitosamente
    public event Action? OnSaveCompleted;
    public event Action? OnCancelRequested;

    public bool IsEditing => _memberId.HasValue;

    public MemberFormViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Inicializa el formulario para nuevo miembro
    /// </summary>
    public async Task InitializeNewAsync()
    {
        _memberId = null;
        Title = "Nuevo Miembro";
        ClearForm();
        await GenerateMemberCodeAsync();
    }

    /// <summary>
    /// Inicializa el formulario para editar miembro existente
    /// </summary>
    public async Task InitializeEditAsync(Guid memberId)
    {
        _memberId = memberId;
        Title = "Editar Miembro";
        await LoadMemberAsync(memberId);
    }

    /// <summary>
    /// Carga los datos del miembro para edición
    /// </summary>
    private async Task LoadMemberAsync(Guid memberId)
    {
        try
        {
            IsLoading = true;

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            var member = await context.Members.FindAsync(memberId);
            if (member != null)
            {
                MemberCode = member.MemberCode;
                FirstName = member.FirstName;
                LastName = member.LastName;
                Email = member.Email ?? string.Empty;
                Phone = member.Phone ?? string.Empty;
                BirthDate = member.BirthDate.HasValue
    ? member.BirthDate.Value.ToDateTime(TimeOnly.MinValue)
    : null;
                SelectedGender = member.Gender;
                Address = member.Address ?? string.Empty;
                EmergencyContact = member.EmergencyContact ?? string.Empty;
                EmergencyPhone = member.EmergencyPhone ?? string.Empty;
                Notes = member.Notes ?? string.Empty;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al cargar miembro: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Genera un código de miembro automático
    /// </summary>
    private async Task GenerateMemberCodeAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            // Generar código: MEM-YYYYMMDD-XXX
            var today = DateTime.Now.ToString("yyyyMMdd");
            var count = context.Members.Count(m => m.MemberCode.StartsWith($"MEM-{today}")) + 1;
            MemberCode = $"MEM-{today}-{count:D3}";
        }
        catch
        {
            MemberCode = $"MEM-{DateTime.Now:yyyyMMddHHmmss}";
        }
    }

    /// <summary>
    /// Limpia el formulario
    /// </summary>
    private void ClearForm()
    {
        MemberCode = string.Empty;
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
        Phone = string.Empty;
        BirthDate = null;
        SelectedGender = null;
        Address = string.Empty;
        EmergencyContact = string.Empty;
        EmergencyPhone = string.Empty;
        Notes = string.Empty;
        ErrorMessage = string.Empty;
    }

    /// <summary>
    /// Valida el formulario
    /// </summary>
    private bool ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(FirstName))
        {
            ErrorMessage = "El nombre es requerido";
            return false;
        }

        if (string.IsNullOrWhiteSpace(LastName))
        {
            ErrorMessage = "El apellido es requerido";
            return false;
        }

        if (!string.IsNullOrWhiteSpace(Email) && !Email.Contains('@'))
        {
            ErrorMessage = "El email no es válido";
            return false;
        }

        ErrorMessage = string.Empty;
        return true;
    }

    /// <summary>
    /// Guarda el miembro
    /// </summary>
    [RelayCommand]
    private async Task SaveAsync()
    {
        if (!ValidateForm()) return;

        try
        {
            IsLoading = true;

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            if (_memberId.HasValue)
            {
                // Editar existente
                var member = await context.Members.FindAsync(_memberId.Value);
                if (member != null)
                {
                    member.FirstName = FirstName.Trim();
                    member.LastName = LastName.Trim();
                    member.Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim();
                    member.Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone.Trim();
                    member.BirthDate = BirthDate.HasValue
    ? DateOnly.FromDateTime(BirthDate.Value)
    : null;
                    member.Gender = SelectedGender;
                    member.Address = string.IsNullOrWhiteSpace(Address) ? null : Address.Trim();
                    member.EmergencyContact = string.IsNullOrWhiteSpace(EmergencyContact) ? null : EmergencyContact.Trim();
                    member.EmergencyPhone = string.IsNullOrWhiteSpace(EmergencyPhone) ? null : EmergencyPhone.Trim();
                    member.Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim();
                    member.UpdatedAt = DateTime.UtcNow;
                }
            }
            else
            {
                // Crear nuevo
                var newMember = new GymManager.Domain.Entities.Member
                {
                    MemberId = Guid.NewGuid(),
                    BranchId = await GetDefaultBranchIdAsync(context),
                    MemberCode = MemberCode,
                    FirstName = FirstName.Trim(),
                    LastName = LastName.Trim(),
                    Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim(),
                    Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone.Trim(),
                    BirthDate = BirthDate.HasValue
    ? DateOnly.FromDateTime(BirthDate.Value)
    : null,
                    Gender = SelectedGender,
                    Address = string.IsNullOrWhiteSpace(Address) ? null : Address.Trim(),
                    EmergencyContact = string.IsNullOrWhiteSpace(EmergencyContact) ? null : EmergencyContact.Trim(),
                    EmergencyPhone = string.IsNullOrWhiteSpace(EmergencyPhone) ? null : EmergencyPhone.Trim(),
                    Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim(),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                context.Members.Add(newMember);
            }

            await context.SaveChangesAsync();

            MessageBox.Show(
                _memberId.HasValue ? "Miembro actualizado correctamente" : "Miembro creado correctamente",
                "Éxito",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            OnSaveCompleted?.Invoke();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al guardar: {ex.Message}";
            MessageBox.Show($"Error al guardar: {ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Obtiene el ID de la sucursal por defecto
    /// </summary>
    private async Task<Guid> GetDefaultBranchIdAsync(GymDbContext context)
    {
        var branch = await context.Branches
            .Where(b => b.IsActive && b.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (branch == null)
        {
            // Crear sucursal por defecto si no existe
            branch = new Branch
            {
                BranchId = Guid.NewGuid(),
                Code = "GYM-001",
                Name = "Sucursal Principal",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Branches.Add(branch);
            await context.SaveChangesAsync();
        }

        return branch.BranchId;
    }

    /// <summary>
    /// Cancela y cierra el formulario
    /// </summary>
    [RelayCommand]
    private void Cancel()
    {
        OnCancelRequested?.Invoke();
    }
}