using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymManager.Domain.Entities;
using GymManager.Domain.Enums;
using GymManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

    // Lista de generos para el ComboBox
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
    /// Carga los datos del miembro para edicion
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
    /// Genera un codigo de miembro automatico UNICO
    /// Formato: MEM-YYYYMMDD-XXX (ej: MEM-20241211-001)
    /// </summary>
    private async Task GenerateMemberCodeAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            var today = DateTime.Now.ToString("yyyyMMdd");
            var prefix = $"MEM-{today}-";

            // Buscar el ultimo codigo del dia para incrementar
            var lastMember = await context.Members
                .Where(m => m.MemberCode.StartsWith(prefix))
                .OrderByDescending(m => m.MemberCode)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastMember != null)
            {
                // Extraer el numero del ultimo codigo (MEM-20241211-005 -> 5)
                var lastCode = lastMember.MemberCode;
                var lastNumberStr = lastCode.Substring(prefix.Length);
                if (int.TryParse(lastNumberStr, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            MemberCode = $"{prefix}{nextNumber:D3}";
        }
        catch (Exception ex)
        {
            // Fallback: usar timestamp unico
            MemberCode = $"MEM-{DateTime.Now:yyyyMMddHHmmss}";
            System.Diagnostics.Debug.WriteLine($"Error generando codigo: {ex.Message}");
        }

        // Asegurar que nunca quede vacio
        if (string.IsNullOrWhiteSpace(MemberCode))
        {
            MemberCode = $"MEM-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
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
        // Validar MemberCode
        if (string.IsNullOrWhiteSpace(MemberCode))
        {
            ErrorMessage = "El codigo de miembro es requerido";
            return false;
        }

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
            ErrorMessage = "El email no es valido";
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
                // Verificar que el codigo no exista
                var existingCode = await context.Members
                    .AnyAsync(m => m.MemberCode == MemberCode);

                if (existingCode)
                {
                    // Regenerar codigo si ya existe
                    await GenerateMemberCodeAsync();
                }

                // Crear nuevo
                var branchId = await GetOrCreateDefaultBranchIdAsync(context);

                var newMember = new GymManager.Domain.Entities.Member
                {
                    MemberId = Guid.NewGuid(),
                    BranchId = branchId,
                    MemberCode = MemberCode.Trim(),
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
                "Exito",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            OnSaveCompleted?.Invoke();
        }
        catch (DbUpdateException dbEx)
        {
            var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;

            // Detectar error de duplicado
            if (innerMessage.Contains("uq_members_member_code") || innerMessage.Contains("duplicate"))
            {
                ErrorMessage = "El codigo de miembro ya existe. Generando nuevo codigo...";
                await GenerateMemberCodeAsync();
                MessageBox.Show($"El codigo ya existia. Se genero uno nuevo: {MemberCode}", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                ErrorMessage = $"Error de BD: {innerMessage}";
                MessageBox.Show($"Error al guardar: {innerMessage}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
    /// Obtiene o crea la sucursal por defecto (y la licencia si es necesario)
    /// </summary>
    private async Task<Guid> GetOrCreateDefaultBranchIdAsync(GymDbContext context)
    {
        // Buscar sucursal existente
        var branch = await context.Branches
            .Where(b => b.IsActive && b.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (branch != null)
        {
            return branch.BranchId;
        }

        // No hay sucursal, necesitamos crear una con su licencia

        // Paso 1: Buscar o crear licencia
        var license = await context.Set<License>()
            .Where(l => l.IsActive)
            .FirstOrDefaultAsync();

        if (license == null)
        {
            // Crear licencia por defecto (Trial)
            license = new License
            {
                LicenseId = Guid.NewGuid(),
                LicenseKey = $"TRIAL-{Guid.NewGuid():N}".Substring(0, 32).ToUpper(),
                HardwareId = Environment.MachineName + "-" + Guid.NewGuid().ToString("N").Substring(0, 8),
                LicenseType = LicenseType.TRIAL,
                MaxBranches = 1,
                MaxUsers = 5,
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Set<License>().Add(license);
            await context.SaveChangesAsync();
        }

        // Paso 2: Crear sucursal con la licencia
        branch = new Branch
        {
            BranchId = Guid.NewGuid(),
            LicenseId = license.LicenseId,
            BranchCode = "GYM-001",
            BranchName = "Sucursal Principal",
            Country = "Mexico",
            IsHeadquarters = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        context.Branches.Add(branch);
        await context.SaveChangesAsync();

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
