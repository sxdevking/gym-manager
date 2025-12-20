using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymManager.Domain.Entities;
using GymManager.Domain.Enums;
using GymManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace GymManager.WPF.ViewModels.Classes;

/// <summary>
/// ViewModel para la vista principal de Clases y Horarios
/// </summary>
public partial class ClassesViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private ObservableCollection<ClassScheduleItem> _classSchedules = new();

    [ObservableProperty]
    private ObservableCollection<ClassTypeItem> _classTypes = new();

    [ObservableProperty]
    private ClassScheduleItem? _selectedSchedule;

    [ObservableProperty]
    private int _selectedDayOfWeek = (int)DateTime.Now.DayOfWeek;

    [ObservableProperty]
    private Guid? _filterClassType;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    // Stats
    [ObservableProperty]
    private int _todayClassesCount;

    [ObservableProperty]
    private int _totalEnrollmentsToday;

    [ObservableProperty]
    private int _availableSpotsToday;

    public string[] DaysOfWeek => new[]
    {
        "Domingo", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado"
    };

    public ClassesViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task InitializeAsync()
    {
        await LoadClassTypesAsync();
        await LoadSchedulesAsync();
        await LoadStatsAsync();
    }

    private async Task LoadClassTypesAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            var types = await context.ClassTypes
                .Where(ct => ct.IsAvailable)
                .OrderBy(ct => ct.Name)
                .Select(ct => new ClassTypeItem
                {
                    ClassTypeId = ct.ClassTypeId,
                    TypeName = ct.Name,
                    Color = ct.Color ?? "#3B82F6",
                    DurationMinutes = ct.DurationMinutes
                })
                .ToListAsync();

            ClassTypes.Clear();
            foreach (var type in types)
            {
                ClassTypes.Add(type);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error cargando tipos: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task LoadSchedulesAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            var query = context.ClassSchedules
                .Include(cs => cs.ClassType)
                .Include(cs => cs.Instructor)
                .Include(cs => cs.ClassEnrollments)
                .Where(cs => cs.IsAvailable)
                .Where(cs => cs.DayOfWeek == SelectedDayOfWeek)
                .AsQueryable();

            // Filtro por tipo de clase
            if (FilterClassType.HasValue)
            {
                query = query.Where(cs => cs.ClassTypeId == FilterClassType.Value);
            }

            var schedules = await query
                .OrderBy(cs => cs.StartTime)
                .Select(cs => new ClassScheduleItem
                {
                    ScheduleId = cs.ScheduleId,
                    ClassName = cs.ClassType.Name,
                    ClassTypeName = cs.ClassType.Name,
                    ClassTypeColor = cs.ClassType.Color ?? "#3B82F6",
                    TrainerName = cs.Instructor != null
                        ? cs.Instructor.FullName
                        : "Sin asignar",
                    StartTime = cs.StartTime,
                    EndTime = cs.EndTime,
                    Capacity = cs.MaxCapacity,
                    EnrolledCount = cs.ClassEnrollments.Count(e => e.Status == EnrollmentStatus.ENROLLED),
                    RoomLocation = cs.Room ?? "Sala principal",
                    DayOfWeek = cs.DayOfWeek
                })
                .ToListAsync();

            ClassSchedules.Clear();
            foreach (var schedule in schedules)
            {
                ClassSchedules.Add(schedule);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al cargar horarios: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadStatsAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            var todayDayOfWeek = (int)DateTime.Now.DayOfWeek;

            var todayClasses = await context.ClassSchedules
                .Include(cs => cs.ClassEnrollments)
                .Where(cs => cs.IsAvailable && cs.DayOfWeek == todayDayOfWeek)
                .ToListAsync();

            TodayClassesCount = todayClasses.Count;
            TotalEnrollmentsToday = todayClasses.Sum(cs =>
                cs.ClassEnrollments.Count(e => e.Status == EnrollmentStatus.ENROLLED));
            AvailableSpotsToday = todayClasses.Sum(cs =>
                cs.MaxCapacity - cs.ClassEnrollments.Count(e => e.Status == EnrollmentStatus.ENROLLED));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error cargando stats: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task SelectDayAsync(string dayOfWeekStr)
    {
        // CommandParameter desde XAML viene como string
        if (int.TryParse(dayOfWeekStr, out int dayOfWeek))
        {
            SelectedDayOfWeek = dayOfWeek;
            await LoadSchedulesAsync();
        }
    }

    [RelayCommand]
    private async Task FilterByTypeAsync(Guid? classTypeId)
    {
        FilterClassType = classTypeId;
        await LoadSchedulesAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        FilterClassType = null;
        SelectedDayOfWeek = (int)DateTime.Now.DayOfWeek;
        await LoadSchedulesAsync();
        await LoadStatsAsync();
    }

    [RelayCommand]
    private async Task NewClassAsync()
    {
        // TODO: Abrir formulario de nueva clase
    }

    [RelayCommand]
    private async Task ViewEnrollmentsAsync()
    {
        if (SelectedSchedule == null) return;
        // TODO: Abrir lista de inscritos
    }

    partial void OnSelectedDayOfWeekChanged(int value)
    {
        _ = LoadSchedulesAsync();
    }
}

/// <summary>
/// Item para la lista de horarios
/// </summary>
public class ClassScheduleItem
{
    public Guid ScheduleId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string ClassTypeName { get; set; } = string.Empty;
    public string ClassTypeColor { get; set; } = "#3B82F6";
    public string TrainerName { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int Capacity { get; set; }
    public int EnrolledCount { get; set; }
    public string RoomLocation { get; set; } = string.Empty;
    public int DayOfWeek { get; set; }

    public string TimeDisplay => $"{StartTime:HH:mm} - {EndTime:HH:mm}";
    public string CapacityDisplay => $"{EnrolledCount}/{Capacity}";
    public int AvailableSpots => Capacity - EnrolledCount;
    public double OccupancyPercent => Capacity > 0 ? (double)EnrolledCount / Capacity * 100 : 0;

    public string OccupancyStatus => OccupancyPercent switch
    {
        >= 100 => "🔴 Lleno",
        >= 80 => "🟠 Casi lleno",
        >= 50 => "🟡 Disponible",
        _ => "🟢 Muchos lugares"
    };
}

/// <summary>
/// Item para tipos de clase
/// </summary>
public class ClassTypeItem
{
    public Guid ClassTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string Color { get; set; } = "#3B82F6";
    public int DurationMinutes { get; set; }

    public string DurationDisplay => $"{DurationMinutes} min";
}