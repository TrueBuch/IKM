using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models;

/// <summary>
/// Сущность маршрута перевозки.
/// Описывает пункт отправления и пункт назначения.
/// </summary>
[Table("routes", Schema = "truck_drivers")]
public class Route : IValidatableObject
{
    /// <summary>
    /// Уникальный идентификатор маршрута.
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Пункт отправления маршрута.
    /// </summary>
    [Required(ErrorMessage = "Пункт отправления обязателен")]
    [Column("origin")]
    [Display(Name = "Пункт отправления")]
    public string Origin { get; set; } = null!;

    /// <summary>
    /// Пункт назначения маршрута.
    /// </summary>
    [Required(ErrorMessage = "Пункт назначения обязателен")]
    [Column("destination")]
    [Display(Name = "Пункт назначения")]
    public string Destination { get; set; } = null!;

    /// <summary>
    /// Полное представление маршрута в виде строки
    /// «Пункт отправления → Пункт назначения».
    /// Не сохраняется в базе данных.
    /// </summary>
    [NotMapped]
    [Display(Name = "Маршрут")]
    public string FullRoute => $"{Origin} → {Destination}";

    /// <summary>
    /// Выполняет дополнительную бизнес-валидацию маршрута.
    /// </summary>
    /// <param name="validationContext">Контекст валидации.</param>
    /// <returns>Коллекция ошибок валидации.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(Origin) &&
            !string.IsNullOrWhiteSpace(Destination) &&
            Origin.Trim().Equals(Destination.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            yield return new ValidationResult(
                "Пункт отправления и пункт назначения не могут совпадать",
                [nameof(Origin), nameof(Destination)]
            );
        }
    }
}
