using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;
using WebApplication1.Helpers;

namespace WebApplication1.Models;

/// <summary>
/// Сущность рейса перевозки.
/// Связывает водителя, грузовик, груз и маршрут,
/// а также содержит плановые и фактические даты выполнения.
/// </summary>
[Table("trips", Schema = "truck_drivers")]
public class Trip : IValidatableObject
{
    /// <summary>
    /// Уникальный идентификатор рейса.
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Идентификатор водителя, выполняющего рейс.
    /// </summary>
    [Required(ErrorMessage = "Выберите водителя")]
    [Column("driver_id")]
    [Display(Name = "Водитель")]
    public int DriverId { get; set; }

    /// <summary>
    /// Идентификатор грузовика, используемого в рейсе.
    /// </summary>
    [Required(ErrorMessage = "Выберите грузовик")]
    [Column("truck_id")]
    [Display(Name = "Грузовик")]
    public int TruckId { get; set; }

    /// <summary>
    /// Идентификатор перевозимого груза.
    /// Один груз может участвовать только в одном рейсе.
    /// </summary>
    [Required(ErrorMessage = "Выберите груз")]
    [Column("cargo_id")]
    [Display(Name = "Груз")]
    public int CargoId { get; set; }

    /// <summary>
    /// Идентификатор маршрута рейса.
    /// </summary>
    [Required(ErrorMessage = "Выберите маршрут")]
    [Column("route_id")]
    [Display(Name = "Маршрут")]
    public int RouteId { get; set; }

    /// <summary>
    /// Плановая дата отправления рейса.
    /// </summary>
    [Required(ErrorMessage = "Укажите дату отправления")]
    [Column("departure_date", TypeName = "timestamp without time zone")]
    [Display(Name = "Дата отправления (план)")]
    [DataType(DataType.Date)]
    public DateTime DepartureDate { get; set; }

    /// <summary>
    /// Плановая дата прибытия рейса.
    /// Не может быть раньше даты отправления.
    /// </summary>
    [Required(ErrorMessage = "Укажите дату прибытия")]
    [Column("arrival_date", TypeName = "timestamp without time zone")]
    [Display(Name = "Дата прибытия (план)")]
    [DataType(DataType.Date)]
    [CompareDate(nameof(DepartureDate),
        ErrorMessage = "Дата прибытия не может быть раньше даты отправления")]
    public DateTime ArrivalDate { get; set; }

    /// <summary>
    /// Фактическая дата отправления рейса.
    /// Заполняется при начале выполнения рейса.
    /// </summary>
    [Column("departure_date_actual", TypeName = "timestamp without time zone")]
    [Display(Name = "Дата отправления (факт)")]
    [DataType(DataType.Date)]
    public DateTime? DepartureDateActual { get; set; }

    /// <summary>
    /// Фактическая дата прибытия рейса.
    /// Заполняется при завершении рейса.
    /// </summary>
    [Column("arrival_date_actual", TypeName = "timestamp without time zone")]
    [Display(Name = "Дата прибытия (факт)")]
    [DataType(DataType.Date)]
    [CompareDate(nameof(DepartureDateActual),
        ErrorMessage = "Фактическая дата прибытия не может быть раньше даты отправления")]
    public DateTime? ArrivalDateActual { get; set; }

    /// <summary>
    /// Текущий статус рейса.
    /// Управляется автоматически на основе фактических дат.
    /// </summary>
    [Column("trip_status")]
    public TripStatuses TripStatus { get; private set; } = TripStatuses.Planned;

    /// <summary>
    /// Навигационное свойство водителя.
    /// </summary>
    public Driver? Driver { get; set; }

    /// <summary>
    /// Навигационное свойство грузовика.
    /// </summary>
    public Truck? Truck { get; set; }

    /// <summary>
    /// Навигационное свойство груза.
    /// </summary>
    public Cargo? Cargo { get; set; }

    /// <summary>
    /// Навигационное свойство маршрута.
    /// </summary>
    public Route? Route { get; set; }

    /// <summary>
    /// Выполняет бизнес-валидацию рейса.
    /// </summary>
    /// <param name="validationContext">Контекст валидации.</param>
    /// <returns>Коллекция ошибок валидации.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DepartureDateActual == null && ArrivalDateActual != null)
        {
            yield return new ValidationResult(
                "Нельзя указать фактическую дату прибытия без даты отправления",
                [nameof(ArrivalDateActual)]
            );
        }

        if (ArrivalDateActual.HasValue && !DepartureDateActual.HasValue)
        {
            yield return new ValidationResult(
                "Для завершённого рейса должна быть указана дата отправления",
                [nameof(DepartureDateActual)]
            );
        }
    }

    /// <summary>
    /// Пересчитывает статус рейса на основе фактических дат.
    /// </summary>
    public void RecalculateStatus()
    {
        if (ArrivalDateActual.HasValue)
            TripStatus = TripStatuses.Completed;
        else if (DepartureDateActual.HasValue)
            TripStatus = TripStatuses.InProgress;
        else
            TripStatus = TripStatuses.Planned;
    }
}

/// <summary>
/// Возможные статусы рейса.
/// </summary>
public enum TripStatuses
{
    /// <summary>
    /// Рейс запланирован, но ещё не начат.
    /// </summary>
    [PgName("Запланирован")]
    [Display(Name = "Запланирован")]
    Planned,

    /// <summary>
    /// Рейс выполняется в данный момент.
    /// </summary>
    [PgName("Выполняется")]
    [Display(Name = "Выполняется")]
    InProgress,

    /// <summary>
    /// Рейс завершён.
    /// </summary>
    [PgName("Завершен")]
    [Display(Name = "Завершен")]
    Completed
}
