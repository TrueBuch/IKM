using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;
using NpgsqlTypes;

namespace WebApplication1.Helpers;

/// <summary>
/// Вспомогательный класс для работы с перечислениями (enum).
/// Предоставляет методы для преобразования enum в элементы
/// пользовательского интерфейса и получения метаданных.
/// </summary>
/// <remarks>
/// Используется для:
/// <list type="bullet">
/// <item>формирования выпадающих списков (<see cref="SelectListItem"/>)</item>
/// <item>получения отображаемых названий enum</item>
/// <item>получения значений enum, используемых в базе данных</item>
/// </list>
/// </remarks>
public static class EnumHelper
{
    /// <summary>
    /// Преобразует перечисление в список элементов
    /// для использования в выпадающем списке.
    /// </summary>
    /// <typeparam name="T">Тип перечисления.</typeparam>
    /// <returns>
    /// Список <see cref="SelectListItem"/>, содержащий все значения enum.
    /// </returns>
    public static List<SelectListItem> FromEnum<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T))
            .Cast<T>()
            .Select(e => new SelectListItem
            {
                Value = e.ToString(),
                Text  = e.GetDisplayName()
            })
            .ToList();
    }

    /// <summary>
    /// Преобразует перечисление в список элементов,
    /// исключая указанные значения.
    /// </summary>
    /// <typeparam name="T">Тип перечисления.</typeparam>
    /// <param name="excluded">
    /// Значения enum, которые необходимо исключить из результата.
    /// </param>
    /// <returns>
    /// Список <see cref="SelectListItem"/> без исключённых значений.
    /// </returns>
    public static List<SelectListItem> FromEnumExcept<T>(params T[] excluded) where T : Enum
    {
        var excludedSet = excluded
            .Select(e => e.ToString())
            .ToHashSet();

        return Enum.GetValues(typeof(T))
            .Cast<T>()
            .Where(e => !excludedSet.Contains(e.ToString()))
            .Select(e => new SelectListItem
            {
                Value = e.ToString(),
                Text  = e.GetDisplayName()
            })
            .ToList();
    }

    /// <summary>
    /// Получает отображаемое имя значения перечисления.
    /// </summary>
    /// <param name="value">Значение enum.</param>
    /// <returns>
    /// Значение атрибута <see cref="DisplayAttribute.Name"/>,
    /// либо строковое представление enum, если атрибут отсутствует.
    /// </returns>
    public static string GetDisplayName(this Enum value)
    {
        return value.GetType()
            .GetMember(value.ToString())
            .First()
            .GetCustomAttribute<DisplayAttribute>()?
            .Name ?? value.ToString();
    }

    /// <summary>
    /// Получает строковое значение перечисления,
    /// используемое для хранения в базе данных PostgreSQL.
    /// </summary>
    /// <param name="value">Значение enum.</param>
    /// <returns>
    /// Значение атрибута <see cref="PgNameAttribute.PgName"/>,
    /// либо строковое представление enum, если атрибут отсутствует.
    /// </returns>
    public static string GetPgName(this Enum value)
    {
        return value.GetType()
            .GetMember(value.ToString())
            .First()
            .GetCustomAttribute<PgNameAttribute>()?
            .PgName
            ?? value.ToString();
    }
}
