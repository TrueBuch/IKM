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
}
