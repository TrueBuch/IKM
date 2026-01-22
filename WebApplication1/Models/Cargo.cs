using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;

namespace WebApplication1.Models;

/// <summary>
/// Сущность груза, предназначенная для перевозки в рамках рейсов.
/// Содержит информацию о параметрах груза, отправителе, получателе и статусе доставки.
/// </summary>
[Table("cargos", Schema = "truck_drivers")]
public class Cargo : IValidatableObject
{
    /// <summary>
    /// Уникальный идентификатор груза.
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Краткое описание груза.
    /// </summary>
    [Required(ErrorMessage = "Описание обязательно")]
    [Column("description")]
    [Display(Name = "Описание")]
    [StringLength(50, ErrorMessage = "Описание не длиннее 50 символов")]
    public string Description { get; set; } = null!;

    /// <summary>
    /// Вес груза в тоннах.
    /// Значение должно быть больше нуля.
    /// </summary>
    [Required(ErrorMessage = "Вес обязателен")]
    [Column("weight_tons")]
    [Display(Name = "Вес (тонн)")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Вес должен быть больше 0")]
    public decimal WeightTons { get; set; }

    /// <summary>
    /// Отправитель груза.
    /// </summary>
    [Required(ErrorMessage = "Отправитель обязателен")]
    [Column("sender")]
    [Display(Name = "Отправитель")]
    public string Sender { get; set; } = null!;

    /// <summary>
    /// Получатель груза.
    /// </summary>
    [Required(ErrorMessage = "Получатель обязателен")]
    [Column("receiver")]
    [Display(Name = "Получатель")]
    public string Receiver { get; set; } = null!;

    /// <summary>
    /// Тип груза (хрупкий, опасный и т.д.).
    /// </summary>
    [Required(ErrorMessage = "Тип груза обязателен")]
    [Column("type")]
    [Display(Name = "Тип груза")]
    public CargoTypes Type { get; set; }

    /// <summary>
    /// Текущий статус груза.
    /// По умолчанию — «Не доставлен».
    /// </summary>
    [Column("cargo_status")]
    [Display(Name = "Статус груза")]
    public CargoStatuses CargoStatus { get; set; } = CargoStatuses.NotDelivered;

    /// <summary>
    /// Пользовательская валидация бизнес-правил груза.
    /// </summary>
    /// <param name="validationContext">Контекст валидации.</param>
    /// <returns>Коллекция ошибок валидации.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(Sender) &&
            !string.IsNullOrWhiteSpace(Receiver) &&
            Sender.Trim().Equals(Receiver.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            yield return new ValidationResult(
                "Отправитель и получатель не могут совпадать",
                [nameof(Sender), nameof(Receiver)]
            );
        }
    }
}

/// <summary>
/// Возможные статусы груза в процессе доставки.
/// </summary>
public enum CargoStatuses
{
    /// <summary>
    /// Груз успешно доставлен получателю.
    /// </summary>
    [PgName("Доставлен")]
    [Display(Name = "Доставлен")]
    Delivered,

    /// <summary>
    /// Груз создан, но ещё не отправлен.
    /// </summary>
    [PgName("Не доставлен")]
    [Display(Name = "Не доставлен")]
    NotDelivered,

    /// <summary>
    /// Груз находится в процессе перевозки.
    /// </summary>
    [PgName("В пути")]
    [Display(Name = "В пути")]
    InTransit
}

/// <summary>
/// Типы грузов, определяющие особенности их перевозки.
/// </summary>
public enum CargoTypes
{
    /// <summary>
    /// Хрупкий груз, требующий осторожной транспортировки.
    /// </summary>
    [PgName("Хрупкий")]
    [Display(Name = "Хрупкий")]
    Fragile,

    /// <summary>
    /// Твёрдый груз без особых условий перевозки.
    /// </summary>
    [PgName("Твердый")]
    [Display(Name = "Твердый")]
    Solid,

    /// <summary>
    /// Опасный груз, требующий специальных условий перевозки.
    /// </summary>
    [PgName("Опасный")]
    [Display(Name = "Опасный")]
    Dangerous,

    /// <summary>
    /// Скоропортящийся груз с ограниченным сроком хранения.
    /// </summary>
    [PgName("Скоропортящиеся")]
    [Display(Name = "Скоропортящиеся")]
    Perishable
}
