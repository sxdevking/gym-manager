using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymManager.Domain.Data;
using GymManager.Domain.Entities;
using GymManager.Domain.Enums;
using GymManager.Infrastructure.Persistence;
using GymManager.WPF.Helpers;
using GymManager.WPF.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GymManager.WPF.ViewModels.Members;

/// <summary>
/// ViewModel para el formulario de miembro (Agregar/Editar)
/// Con ciudades dinamicas por estado, busqueda de referidos y validaciones
/// </summary>
public partial class MemberFormViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    private Guid? _memberId;
    private string? _originalPhotoPath;
    private string? _defaultState; // Estado por defecto de la sucursal

    #region Propiedades de Estado

    [ObservableProperty]
    private string _title = "Nuevo Miembro";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private int _selectedTabIndex;

    #endregion

    #region Informacion Basica

    [ObservableProperty]
    private string _memberCode = string.Empty;

    private string _firstName = string.Empty;
    public string FirstName
    {
        get => _firstName;
        set
        {
            if (SetProperty(ref _firstName, value))
            {
                ValidateFirstName();
            }
        }
    }

    private string _lastName = string.Empty;
    public string LastName
    {
        get => _lastName;
        set
        {
            if (SetProperty(ref _lastName, value))
            {
                ValidateLastName();
            }
        }
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set
        {
            // Limpiar espacios automaticamente
            var cleaned = value?.Replace(" ", "") ?? string.Empty;
            if (SetProperty(ref _email, cleaned))
            {
                ValidateEmail();
            }
        }
    }

    [ObservableProperty]
    private string _phone = string.Empty;

    [ObservableProperty]
    private string _mobilePhone = string.Empty;

    #endregion

    #region Datos Personales

    [ObservableProperty]
    private DateTime? _birthDate;

    [ObservableProperty]
    private Gender? _selectedGender;

    [ObservableProperty]
    private string _idDocumentType = string.Empty;

    [ObservableProperty]
    private string _idDocumentNumber = string.Empty;

    [ObservableProperty]
    private string _photoPath = string.Empty;

    [ObservableProperty]
    private BitmapImage? _photoPreview;

    #endregion

    #region Direccion

    [ObservableProperty]
    private string _address = string.Empty;

    private string _state = string.Empty;
    public string State
    {
        get => _state;
        set
        {
            if (SetProperty(ref _state, value))
            {
                // Actualizar ciudades cuando cambia el estado
                UpdateCitiesForState(value);
            }
        }
    }

    [ObservableProperty]
    private string _city = string.Empty;

    [ObservableProperty]
    private ObservableCollection<string> _availableCities = new();

    [ObservableProperty]
    private string _postalCode = string.Empty;

    #endregion

    #region Contacto de Emergencia

    [ObservableProperty]
    private string _emergencyContactName = string.Empty;

    [ObservableProperty]
    private string _emergencyContactPhone = string.Empty;

    [ObservableProperty]
    private string _emergencyContactRelationship = string.Empty;

    #endregion

    #region Informacion Adicional

    [ObservableProperty]
    private string _medicalNotes = string.Empty;

    [ObservableProperty]
    private string _notes = string.Empty;

    [ObservableProperty]
    private Guid? _referredByMemberId;

    [ObservableProperty]
    private bool _isActive = true;

    // Para mostrar el nombre del referido seleccionado
    [ObservableProperty]
    private string _referredByMemberDisplay = string.Empty;

    // Texto de busqueda para filtrar miembros
    private string _memberSearchText = string.Empty;
    public string MemberSearchText
    {
        get => _memberSearchText;
        set
        {
            if (SetProperty(ref _memberSearchText, value))
            {
                FilterMembers(value);
            }
        }
    }

    #endregion

    #region Errores de Validacion

    [ObservableProperty]
    private bool _hasFirstNameError;

    [ObservableProperty]
    private bool _hasLastNameError;

    [ObservableProperty]
    private bool _hasEmailError;

    [ObservableProperty]
    private string _firstNameErrorMessage = string.Empty;

    [ObservableProperty]
    private string _lastNameErrorMessage = string.Empty;

    [ObservableProperty]
    private string _emailErrorMessage = string.Empty;

    #endregion

    #region Listas para ComboBoxes

    public IEnumerable<Gender> Genders => Enum.GetValues<Gender>();

    // Lista completa de miembros (sin filtrar)
    private List<Member> _allMembers = new();

    // Lista filtrada para mostrar en ComboBox
    [ObservableProperty]
    private ObservableCollection<Member> _availableMembers = new();

    public IEnumerable<string> MexicanStates => MexicoGeographyData.States;

    public IEnumerable<string> DocumentTypes => new[]
    {
        "INE",
        "Pasaporte",
        "Licencia de Conducir",
        "Cedula Profesional",
        "Otro"
    };

    public IEnumerable<string> Relationships => new[]
    {
        "Padre/Madre",
        "Hijo/Hija",
        "Esposo/Esposa",
        "Hermano/Hermana",
        "Amigo/Amiga",
        "Otro"
    };

    #endregion

    #region Eventos

    public event Action? OnSaveCompleted;
    public event Action? OnCancelRequested;

    #endregion

    public bool IsEditing => _memberId.HasValue;

    public MemberFormViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    #region Inicializacion

    public async Task InitializeNewAsync()
    {
        _memberId = null;
        Title = "Nuevo Miembro";
        ClearForm();
        await LoadDefaultStateFromBranchAsync();
        await GenerateMemberCodeAsync();
        await LoadAvailableMembersAsync();
    }

    public async Task InitializeEditAsync(Guid memberId)
    {
        _memberId = memberId;
        Title = "Editar Miembro";
        await LoadDefaultStateFromBranchAsync();
        await LoadAvailableMembersAsync();
        await LoadMemberAsync(memberId);
    }

    /// <summary>
    /// Carga el estado por defecto desde la sucursal
    /// </summary>
    private async Task LoadDefaultStateFromBranchAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            var branch = await context.Branches
                .Where(b => b.IsActive && b.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (branch != null)
            {
                // Intentar obtener el estado de la sucursal si existe la propiedad
                // Si no existe, usar el estado por defecto
                try
                {
                    var stateProperty = branch.GetType().GetProperty("State");
                    if (stateProperty != null)
                    {
                        _defaultState = stateProperty.GetValue(branch)?.ToString() ?? "Quintana Roo";
                    }
                    else
                    {
                        _defaultState = "Quintana Roo";
                    }
                }
                catch
                {
                    _defaultState = "Quintana Roo";
                }
            }
            else
            {
                _defaultState = "Quintana Roo";
            }
        }
        catch
        {
            _defaultState = "Quintana Roo";
        }
    }

    /// <summary>
    /// Actualiza la lista de ciudades segun el estado seleccionado
    /// </summary>
    private void UpdateCitiesForState(string? state)
    {
        AvailableCities.Clear();

        var cities = MexicoGeographyData.GetCities(state);
        foreach (var city in cities)
        {
            AvailableCities.Add(city);
        }

        // Si la ciudad actual no esta en la lista, limpiarla
        if (!string.IsNullOrEmpty(City) && !AvailableCities.Contains(City))
        {
            City = string.Empty;
        }
    }

    private async Task LoadAvailableMembersAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            _allMembers = await context.Members
                .Where(m => m.IsActive && m.DeletedAt == null)
                .OrderBy(m => m.FirstName)
                .ThenBy(m => m.LastName)
                .ToListAsync();

            // Excluir el miembro actual si estamos editando
            if (_memberId.HasValue)
            {
                _allMembers = _allMembers.Where(m => m.MemberId != _memberId.Value).ToList();
            }

            // Inicialmente mostrar todos
            FilterMembers(string.Empty);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error cargando miembros: {ex.Message}");
        }
    }

    /// <summary>
    /// Filtra la lista de miembros segun el texto de busqueda
    /// </summary>
    private void FilterMembers(string? searchText)
    {
        AvailableMembers.Clear();

        var filtered = _allMembers.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var search = searchText.ToLower();
            filtered = filtered.Where(m =>
                m.FirstName.ToLower().Contains(search) ||
                m.LastName.ToLower().Contains(search) ||
                m.MemberCode.ToLower().Contains(search) ||
                (m.Email?.ToLower().Contains(search) ?? false));
        }

        foreach (var member in filtered.Take(20)) // Limitar a 20 resultados
        {
            AvailableMembers.Add(member);
        }
    }

    private async Task LoadMemberAsync(Guid memberId)
    {
        try
        {
            IsLoading = true;

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            var member = await context.Members
                .Include(m => m.ReferredByMember)
                .FirstOrDefaultAsync(m => m.MemberId == memberId);

            if (member != null)
            {
                MemberCode = member.MemberCode;
                _firstName = member.FirstName;
                OnPropertyChanged(nameof(FirstName));
                _lastName = member.LastName;
                OnPropertyChanged(nameof(LastName));
                _email = member.Email ?? string.Empty;
                OnPropertyChanged(nameof(Email));
                Phone = member.Phone ?? string.Empty;
                MobilePhone = member.MobilePhone ?? string.Empty;

                BirthDate = member.BirthDate.HasValue
                    ? member.BirthDate.Value.ToDateTime(TimeOnly.MinValue)
                    : null;
                SelectedGender = member.Gender;
                IdDocumentType = member.IdDocumentType ?? string.Empty;
                IdDocumentNumber = member.IdDocumentNumber ?? string.Empty;

                _originalPhotoPath = member.PhotoPath;
                PhotoPath = member.PhotoPath ?? string.Empty;
                LoadPhotoPreview();

                Address = member.Address ?? string.Empty;

                // Cargar estado primero (esto actualiza las ciudades)
                _state = member.State ?? string.Empty;
                OnPropertyChanged(nameof(State));
                UpdateCitiesForState(_state);

                City = member.City ?? string.Empty;
                PostalCode = member.PostalCode ?? string.Empty;

                EmergencyContactName = member.EmergencyContactName ?? string.Empty;
                EmergencyContactPhone = member.EmergencyContactPhone ?? string.Empty;
                EmergencyContactRelationship = member.EmergencyContactRelationship ?? string.Empty;

                MedicalNotes = member.MedicalNotes ?? string.Empty;
                Notes = member.Notes ?? string.Empty;
                ReferredByMemberId = member.ReferredByMemberId;

                // Mostrar nombre del referido
                if (member.ReferredByMember != null)
                {
                    ReferredByMemberDisplay = $"{member.ReferredByMember.MemberCode} - {member.ReferredByMember.FirstName} {member.ReferredByMember.LastName}";
                }

                IsActive = member.IsActive;
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

    private async Task GenerateMemberCodeAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            var today = DateTime.Now.ToString("yyyyMMdd");
            var prefix = $"MEM-{today}-";

            var lastMember = await context.Members
                .Where(m => m.MemberCode.StartsWith(prefix))
                .OrderByDescending(m => m.MemberCode)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastMember != null)
            {
                var lastCode = lastMember.MemberCode;
                var lastNumberStr = lastCode.Substring(prefix.Length);
                if (int.TryParse(lastNumberStr, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            MemberCode = $"{prefix}{nextNumber:D3}";
        }
        catch
        {
            MemberCode = $"MEM-{DateTime.Now:yyyyMMddHHmmss}";
        }

        if (string.IsNullOrWhiteSpace(MemberCode))
        {
            MemberCode = $"MEM-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
        }
    }

    private void ClearForm()
    {
        MemberCode = string.Empty;
        _firstName = string.Empty;
        OnPropertyChanged(nameof(FirstName));
        _lastName = string.Empty;
        OnPropertyChanged(nameof(LastName));
        _email = string.Empty;
        OnPropertyChanged(nameof(Email));
        Phone = string.Empty;
        MobilePhone = string.Empty;

        BirthDate = null;
        SelectedGender = null;
        IdDocumentType = string.Empty;
        IdDocumentNumber = string.Empty;
        PhotoPath = string.Empty;
        PhotoPreview = null;
        _originalPhotoPath = null;

        Address = string.Empty;

        // Establecer estado por defecto de la sucursal
        _state = _defaultState ?? string.Empty;
        OnPropertyChanged(nameof(State));
        UpdateCitiesForState(_state);

        City = string.Empty;
        PostalCode = string.Empty;

        EmergencyContactName = string.Empty;
        EmergencyContactPhone = string.Empty;
        EmergencyContactRelationship = string.Empty;

        MedicalNotes = string.Empty;
        Notes = string.Empty;
        ReferredByMemberId = null;
        ReferredByMemberDisplay = string.Empty;
        MemberSearchText = string.Empty;
        IsActive = true;

        ClearAllErrors();
        SelectedTabIndex = 0;
    }

    private void ClearAllErrors()
    {
        ErrorMessage = string.Empty;
        HasFirstNameError = false;
        HasLastNameError = false;
        HasEmailError = false;
        FirstNameErrorMessage = string.Empty;
        LastNameErrorMessage = string.Empty;
        EmailErrorMessage = string.Empty;
    }

    #endregion

    #region Validacion

    private void ValidateFirstName()
    {
        if (string.IsNullOrWhiteSpace(FirstName))
        {
            HasFirstNameError = true;
            FirstNameErrorMessage = "El nombre es requerido";
        }
        else
        {
            HasFirstNameError = false;
            FirstNameErrorMessage = string.Empty;
        }
    }

    private void ValidateLastName()
    {
        if (string.IsNullOrWhiteSpace(LastName))
        {
            HasLastNameError = true;
            LastNameErrorMessage = "El apellido es requerido";
        }
        else
        {
            HasLastNameError = false;
            LastNameErrorMessage = string.Empty;
        }
    }

    private void ValidateEmail()
    {
        if (!string.IsNullOrWhiteSpace(Email) && !ValidationHelper.IsValidEmail(Email))
        {
            HasEmailError = true;
            EmailErrorMessage = "Email invalido (sin espacios, formato: ejemplo@dominio.com)";
        }
        else
        {
            HasEmailError = false;
            EmailErrorMessage = string.Empty;
        }
    }

    private bool ValidateForm()
    {
        ClearAllErrors();
        var isValid = true;

        if (string.IsNullOrWhiteSpace(MemberCode))
        {
            ErrorMessage = "El codigo de miembro es requerido";
            SelectedTabIndex = 0;
            return false;
        }

        if (string.IsNullOrWhiteSpace(FirstName))
        {
            HasFirstNameError = true;
            FirstNameErrorMessage = "El nombre es requerido";
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(LastName))
        {
            HasLastNameError = true;
            LastNameErrorMessage = "El apellido es requerido";
            isValid = false;
        }

        if (!string.IsNullOrWhiteSpace(Email))
        {
            if (Email.Contains(' '))
            {
                HasEmailError = true;
                EmailErrorMessage = "El email no puede contener espacios";
                isValid = false;
            }
            else if (!ValidationHelper.IsValidEmail(Email))
            {
                HasEmailError = true;
                EmailErrorMessage = "Formato de email invalido";
                isValid = false;
            }
        }

        if (!isValid)
        {
            ErrorMessage = "Por favor corrija los errores marcados en rojo";
            SelectedTabIndex = 0;
        }

        return isValid;
    }

    #endregion

    #region Manejo de Fotos

    private void LoadPhotoPreview()
    {
        PhotoPreview = null;

        if (string.IsNullOrWhiteSpace(PhotoPath))
            return;

        try
        {
            var fullPath = MemberPhotoService.GetFullPhotoPath(PhotoPath);

            if (File.Exists(fullPath))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(fullPath, UriKind.Absolute);
                bitmap.DecodePixelWidth = 200;
                bitmap.EndInit();
                bitmap.Freeze();
                PhotoPreview = bitmap;
            }
            else if (Path.IsPathRooted(PhotoPath) && File.Exists(PhotoPath))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(PhotoPath, UriKind.Absolute);
                bitmap.DecodePixelWidth = 200;
                bitmap.EndInit();
                bitmap.Freeze();
                PhotoPreview = bitmap;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error cargando preview de foto: {ex.Message}");
        }
    }

    [RelayCommand]
    private void SelectPhoto()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "Seleccionar Foto del Miembro",
            Filter = "Imagenes|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Todos los archivos|*.*",
            CheckFileExists = true
        };

        if (dialog.ShowDialog() == true)
        {
            PhotoPath = dialog.FileName;
            LoadPhotoPreview();
        }
    }

    [RelayCommand]
    private void ClearPhoto()
    {
        PhotoPath = string.Empty;
        PhotoPreview = null;
    }

    private string? ProcessAndSavePhoto(Guid memberId)
    {
        if (string.IsNullOrWhiteSpace(PhotoPath))
            return null;

        if (PhotoPath == _originalPhotoPath)
            return _originalPhotoPath;

        if (!Path.IsPathRooted(PhotoPath))
            return PhotoPath;

        try
        {
            var savedFileName = MemberPhotoService.SaveMemberPhoto(PhotoPath, memberId);
            return savedFileName;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error guardando foto: {ex.Message}");
            MessageBox.Show($"No se pudo guardar la foto: {ex.Message}", "Advertencia",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return null;
        }
    }

    #endregion

    #region Comandos de Referido

    /// <summary>
    /// Selecciona un miembro como referido
    /// </summary>
    [RelayCommand]
    private void SelectReferredMember(Member? member)
    {
        if (member != null)
        {
            ReferredByMemberId = member.MemberId;
            ReferredByMemberDisplay = $"{member.MemberCode} - {member.FirstName} {member.LastName}";
        }
    }

    /// <summary>
    /// Limpia el miembro referido seleccionado
    /// </summary>
    [RelayCommand]
    private void ClearReferredMember()
    {
        ReferredByMemberId = null;
        ReferredByMemberDisplay = string.Empty;
        MemberSearchText = string.Empty;
    }

    #endregion

    #region Comandos Principales

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (!ValidateForm()) return;

        try
        {
            IsLoading = true;

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            Guid currentMemberId;

            if (_memberId.HasValue)
            {
                currentMemberId = _memberId.Value;
                var member = await context.Members.FindAsync(_memberId.Value);
                if (member != null)
                {
                    UpdateMemberFromForm(member);
                    member.PhotoPath = ProcessAndSavePhoto(currentMemberId);
                    member.UpdatedAt = DateTime.UtcNow;
                }
            }
            else
            {
                var existingCode = await context.Members
                    .AnyAsync(m => m.MemberCode == MemberCode);

                if (existingCode)
                {
                    await GenerateMemberCodeAsync();
                }

                var branchId = await GetOrCreateDefaultBranchIdAsync(context);
                currentMemberId = Guid.NewGuid();

                var newMember = new Member
                {
                    MemberId = currentMemberId,
                    BranchId = branchId,
                    MemberCode = MemberCode.Trim(),
                    RegistrationDate = DateOnly.FromDateTime(DateTime.Now),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                UpdateMemberFromForm(newMember);
                newMember.PhotoPath = ProcessAndSavePhoto(currentMemberId);
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

            if (innerMessage.Contains("uq_members_member_code") || innerMessage.Contains("duplicate"))
            {
                ErrorMessage = "El codigo de miembro ya existe";
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

    private void UpdateMemberFromForm(Member member)
    {
        member.FirstName = ValidationHelper.FormatName(FirstName);
        member.LastName = ValidationHelper.FormatName(LastName);
        member.Email = string.IsNullOrWhiteSpace(Email) ? null : ValidationHelper.CleanEmail(Email);
        member.Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone.Trim();
        member.MobilePhone = string.IsNullOrWhiteSpace(MobilePhone) ? null : MobilePhone.Trim();

        member.BirthDate = BirthDate.HasValue ? DateOnly.FromDateTime(BirthDate.Value) : null;
        member.Gender = SelectedGender;
        member.IdDocumentType = NullIfEmpty(IdDocumentType);
        member.IdDocumentNumber = NullIfEmpty(IdDocumentNumber);

        member.Address = NullIfEmpty(ValidationHelper.CleanText(Address));
        member.State = NullIfEmpty(State);
        member.City = NullIfEmpty(ValidationHelper.CleanText(City));
        member.PostalCode = NullIfEmpty(PostalCode?.Trim());

        member.EmergencyContactName = NullIfEmpty(ValidationHelper.CleanText(EmergencyContactName));
        member.EmergencyContactPhone = NullIfEmpty(EmergencyContactPhone?.Trim());
        member.EmergencyContactRelationship = NullIfEmpty(EmergencyContactRelationship);

        member.MedicalNotes = NullIfEmpty(MedicalNotes?.Trim());
        member.Notes = NullIfEmpty(Notes?.Trim());
        member.ReferredByMemberId = ReferredByMemberId;
        member.IsActive = IsActive;
    }

    private static string? NullIfEmpty(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value;

    private async Task<Guid> GetOrCreateDefaultBranchIdAsync(GymDbContext context)
    {
        var branch = await context.Branches
            .Where(b => b.IsActive && b.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (branch != null)
            return branch.BranchId;

        var license = await context.Set<License>()
            .Where(l => l.IsActive)
            .FirstOrDefaultAsync();

        if (license == null)
        {
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

    [RelayCommand]
    private void Cancel()
    {
        OnCancelRequested?.Invoke();
    }

    #endregion
}