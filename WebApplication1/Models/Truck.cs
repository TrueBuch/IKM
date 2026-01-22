using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;

namespace WebApplication1.Models;

/// <summary>
/// Сущность грузовика.
/// Описывает транспортное средство, используемое для перевозки грузов.
/// </summary>
[Table("trucks", Schema = "truck_drivers")]
public class Truck : IValidatableObject
{
    /// <summary>
    /// Уникальный идентификатор грузовика.
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Марка грузовика.
    /// </summary>
    [Required(ErrorMessage = "Марка обязательна")]
    [Column("brand")]
    [Display(Name = "Марка")]
    public string Brand { get; set; } = null!;

    /// <summary>
    /// Модель грузовика.
    /// </summary>
    [Required(ErrorMessage = "Модель обязательна")]
    [Column("model")]
    [Display(Name = "Модель")]
    public string Model { get; set; } = null!;

    /// <summary>
    /// Год выпуска грузовика.
    /// </summary>
    [Column("year")]
    [Display(Name = "Год выпуска")]
    public int? Year { get; set; }

    /// <summary>
    /// Максимальная грузоподъёмность грузовика в тоннах.
    /// </summary>
    [Required(ErrorMessage = "Грузоподъёмность обязательна")]
    [Column("capasity_tons")]
    [Display(Name = "Грузоподъёмность (т)")]
    [Range(0.01, double.MaxValue,
        ErrorMessage = "Грузоподъёмность должна быть больше 0")]
    public decimal CapacityTons { get; set; }

    /// <summary>
    /// Регистрационный номер грузовика.
    /// </summary>
    [Required(ErrorMessage = "Гос. номер обязателен")]
    [Column("plate_number")]
    [Display(Name = "Гос. номер")]
    public string PlateNumber { get; set; } = null!;

    /// <summary>
    /// Текущий статус грузовика.
    /// </summary>
    [Column("truck_status")]
    [Display(Name = "Статус")]
    public TruckStatuses TruckStatus { get; set; } = TruckStatuses.Free;

    /// <summary>
    /// Выполняет бизнес-валидацию грузовика.
    /// </summary>
    /// <param name="validationContext">Контекст валидации.</param>
    /// <returns>Коллекция ошибок валидации.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Year.HasValue && Year.Value > DateTime.Now.Year)
        {
            yield return new ValidationResult(
                "Год выпуска не может быть больше текущего",
                [nameof(Year)]
            );
        }
    }
}

/// <summary>
/// Возможные статусы грузовика.
/// </summary>
public enum TruckStatuses
{
    /// <summary>
    /// Грузовик участвует в рейсе.
    /// </summary>
    [PgName("В рейсе")]
    [Display(Name = "В рейсе")]
    InTrip,

    /// <summary>
    /// Грузовик находится на ремонте и недоступен для рейсов.
    /// </summary>
    [PgName("На ремонте")]
    [Display(Name = "На ремонте")]
    InRepair,

    /// <summary>
    /// Грузовик свободен и доступен для назначения в рейс.
    /// </summary>
    [PgName("Свободен")]
    [Display(Name = "Свободен")]
    Free
}
