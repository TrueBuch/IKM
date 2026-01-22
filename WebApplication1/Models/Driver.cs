using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;

namespace WebApplication1.Models;

/// <summary>
/// Сущность водителя грузового транспорта.
/// Содержит персональные данные, контактную информацию и текущий статус занятости.
/// </summary>
[Table("drivers", Schema = "truck_drivers")]
public class Driver : IValidatableObject
{
    /// <summary>
    /// Уникальный идентификатор водителя.
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Фамилия водителя.
    /// Может содержать только буквы и дефис.
    /// </summary>
    [Required(ErrorMessage = "Фамилия обязательна")]
    [Column("surname")]
    [Display(Name = "Фамилия")]
    [RegularExpression(@"^[А-Яа-яA-Za-z\-]+$",
        ErrorMessage = "Фамилия может содержать только буквы и дефис")]
    public string Surname { get; set; } = null!;

    /// <summary>
    /// Имя водителя.
    /// Может содержать только буквы и дефис.
    /// </summary>
    [Required(ErrorMessage = "Имя обязательно")]
    [Column("name")]
    [Display(Name = "Имя")]
    [RegularExpression(@"^[А-Яа-яA-Za-z\-]+$",
        ErrorMessage = "Имя может содержать только буквы и дефис")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Отчество водителя (необязательное поле).
    /// </summary>
    [Column("middle_name")]
    [Display(Name = "Отчество")]
    [RegularExpression(@"^[А-Яа-яA-Za-z\-]*$",
        ErrorMessage = "Отчество может содержать только буквы и дефис")]
    public string? MiddleName { get; set; }

    /// <summary>
    /// Дата рождения водителя.
    /// </summary>
    [Column("birth_date", TypeName = "date")]
    [Display(Name = "Дата рождения")]
    [DataType(DataType.Date)]
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// Контактный номер телефона водителя.
    /// Должен содержать только цифры.
    /// </summary>
    [Required(ErrorMessage = "Телефон обязателен")]
    [Column("phone_number")]
    [Display(Name = "Телефон")]
    [RegularExpression(@"^\d+$",
        ErrorMessage = "Телефон должен содержать только цифры")]
    public string PhoneNumber { get; set; } = null!;

    /// <summary>
    /// Номер водительского удостоверения.
    /// </summary>
    [Required(ErrorMessage = "Номер удостоверения обязателен")]
    [Column("license_number")]
    [Display(Name = "Номер удостоверения")]
    [RegularExpression(@"^[A-ZА-Я0-9\-]+$",
        ErrorMessage = "Номер удостоверения содержит недопустимые символы")]
    public string LicenseNumber { get; set; } = null!;

    /// <summary>
    /// Текущий статус водителя (свободен, в рейсе, в отпуске и т.д.).
    /// По умолчанию — «Свободен».
    /// </summary>
    [Column("driver_status")]
    [Display(Name = "Статус водителя")]
    public DriverStatuses DriverStatus { get; set; } = DriverStatuses.Free;

    /// <summary>
    /// Полное имя водителя (ФИО).
    /// Вычисляемое свойство, не сохраняется в базе данных.
    /// </summary>
    [NotMapped]
    [Display(Name = "ФИО")]
    public string FullName =>
        string.IsNullOrWhiteSpace(MiddleName)
            ? $"{Surname} {Name}"
            : $"{Surname} {Name} {MiddleName}";

    /// <summary>
    /// Пользовательская бизнес-валидация данных водителя.
    /// </summary>
    /// <param name="validationContext">Контекст валидации.</param>
    /// <returns>Коллекция ошибок валидации.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (BirthDate.HasValue && BirthDate.Value.Date > DateTime.Today)
        {
            yield return new ValidationResult(
                "Дата рождения не может быть в будущем",
                [nameof(BirthDate)]
            );
        }
    }
}

/// <summary>
/// Возможные статусы водителя.
/// </summary>
public enum DriverStatuses
{
    /// <summary>
    /// Водитель свободен и доступен для назначения в рейс.
    /// </summary>
    [PgName("Свободен")]
    [Display(Name = "Свободен")]
    Free,

    /// <summary>
    /// Водитель находится в отпуске.
    /// </summary>
    [PgName("В отпуске")]
    [Display(Name = "В отпуске")]
    Vacation,

    /// <summary>
    /// Водитель участвует в рейсе.
    /// </summary>
    [PgName("В рейсе")]
    [Display(Name = "В рейсе")]
    Trip,

    /// <summary>
    /// Водитель находится на больничном.
    /// </summary>
    [PgName("На больничном")]
    [Display(Name = "На больничном")]
    Sick
}
