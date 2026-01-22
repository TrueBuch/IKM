using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;

namespace WebApplication1.Controllers;

/// <summary>
/// Контроллер для управления маршрутами.
/// Обеспечивает операции просмотра, добавления,
/// редактирования и удаления маршрутов.
/// </summary>
public class RoutesController : BaseController<Models.Route>
{
    /// <summary>
    /// Инициализирует новый экземпляр контроллера маршрутов.
    /// </summary>
    /// <param name="context">Контекст базы данных приложения.</param>
    public RoutesController(AppDbContext context) : base(context) { }

    /// <summary>
    /// Подтверждает удаление маршрута.
    /// Удаление запрещено, если маршрут используется в рейсах.
    /// </summary>
    /// <param name="id">Идентификатор маршрута.</param>
    /// <returns>
    /// Представление подтверждения удаления с ошибкой
    /// либо перенаправление на список маршрутов.
    /// </returns>
    public override IActionResult DeleteConfirmed(int id)
    {
        bool hasTrips = _context.Trips.Any(t => t.RouteId == id);

        if (hasTrips)
        {
            ModelState.AddModelError(string.Empty,
                "Нельзя удалить маршрут, который используется в рейсах.");

            var route = _context.Routes.Find(id);
            return View("Delete", route);
        }

        return base.DeleteConfirmed(id);
    }
}
