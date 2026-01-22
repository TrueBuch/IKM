using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Helpers;
using WebApplication1.Models;

/// <summary>
/// Контроллер для управления грузовиками.
/// Обеспечивает стандартные CRUD-операции и
/// загрузку допустимых статусов для форм создания и редактирования.
/// </summary>
public class TrucksController : BaseController<Truck>
{
    /// <summary>
    /// Инициализирует новый экземпляр контроллера грузовиков.
    /// </summary>
    /// <param name="context">Контекст базы данных приложения.</param>
    public TrucksController(AppDbContext context) : base(context) { }

    /// <summary>
    /// Загружает данные, необходимые для представлений.
    /// Формирует списки статусов грузовиков:
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// <c>EditableStatuses</c> — статусы, доступные для выбора пользователем
    /// (исключая статус «В рейсе»).
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <c>AllStatuses</c> — полный список статусов грузовиков
    /// для отображения текущего состояния.
    /// </description>
    /// </item>
    /// </list>
    /// </summary>
    protected override void LoadViewData()
    {
        ViewBag.EditableStatuses = EnumHelper.FromEnumExcept(TruckStatuses.InTrip);
        ViewBag.AllStatuses = EnumHelper.FromEnum<TruckStatuses>();
    }
}
