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

    /// <summary>
    /// Подтверждает удаление грузовика.
    /// Удаление запрещено, если грузовик используется в рейсах.
    /// </summary>
    /// <param name="id">Идентификатор грузовика.</param>
    /// <returns>
    /// Представление подтверждения удаления с ошибкой
    /// либо перенаправление на список грузовиков.
    /// </returns>
    public override IActionResult DeleteConfirmed(int id)
    {
        bool hasTrips = _context.Trips.Any(t => t.TruckId == id);

        if (hasTrips)
        {
            ModelState.AddModelError(string.Empty,
                "Нельзя удалить грузовик, который используется в рейсах.");

            var truck = _context.Trucks.Find(id);
            return View("Delete", truck);
        }

        return base.DeleteConfirmed(id);
    }
}
