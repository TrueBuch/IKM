using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Helpers;

/// <summary>
/// Атрибут валидации для сравнения двух дат.
/// Проверяет, что значение текущего свойства
/// не меньше значения другого свойства объекта.
/// </summary>
/// <remarks>
/// Используется, например, для проверки того,
/// что дата прибытия не раньше даты отправления.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class CompareDateAttribute : ValidationAttribute
{
    private readonly string _comparisonProperty;

    /// <summary>
    /// Инициализирует новый экземпляр атрибута сравнения дат.
    /// </summary>
    /// <param name="comparisonProperty">
    /// Имя свойства, с которым необходимо сравнивать текущую дату.
    /// </param>
    public CompareDateAttribute(string comparisonProperty)
    {
        _comparisonProperty = comparisonProperty;
    }

    /// <summary>
    /// Выполняет проверку корректности значения даты.
    /// </summary>
    /// <param name="value">Значение текущего свойства.</param>
    /// <param name="validationContext">
    /// Контекст валидации, содержащий объект и его метаданные.
    /// </param>
    /// <returns>
    /// <see cref="ValidationResult.Success"/>, если значение корректно;
    /// иначе объект <see cref="ValidationResult"/> с описанием ошибки.
    /// </returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var currentDate = value as DateTime?;

        var property = validationContext.ObjectType
            .GetProperty(_comparisonProperty);

        if (property == null)
        {
            return new ValidationResult($"Свойство {_comparisonProperty} не найдено");
        }

        var comparisonValue =
            property.GetValue(validationContext.ObjectInstance) as DateTime?;

        if (!currentDate.HasValue || !comparisonValue.HasValue)
            return ValidationResult.Success;

        if (currentDate < comparisonValue)
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}
