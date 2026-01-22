using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Helpers;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

/// <summary>
/// Контроллер для управления грузами.
/// Обеспечивает просмотр, добавление, редактирование и удаление грузов.
/// </summary>
public class CargosController : BaseController<Cargo>
{
    /// <summary>
    /// Инициализирует новый экземпляр контроллера грузов.
    /// </summary>
    /// <param name="context">Контекст базы данных приложения.</param>
    public CargosController(AppDbContext context) : base(context) { }

    /// <summary>
    /// Загружает данные, необходимые для представлений грузов.
    /// Используется для заполнения списков типов и статусов грузов.
    /// </summary>
    protected override void LoadViewData()
    {
        ViewBag.Types = EnumHelper.FromEnum<CargoTypes>();
        ViewBag.Statuses = EnumHelper.FromEnum<CargoStatuses>();
    }

    /// <summary>
    /// Подтверждает удаление груза.
    /// Удаление запрещено, если груз связан с рейсами.
    /// </summary>
    /// <param name="id">Идентификатор груза.</param>
    /// <returns>
    /// Представление подтверждения удаления с ошибкой
    /// либо перенаправление на список грузов.
    /// </returns>
    public override IActionResult DeleteConfirmed(int id)
    {
        bool hasTrips = _context.Trips.Any(t => t.CargoId == id);

        if (hasTrips)
        {
            ModelState.AddModelError(string.Empty,
                "Нельзя удалить груз, который используется в рейсах.");

            var cargo = _context.Cargos.Find(id);
            return View("Delete", cargo);
        }

        return base.DeleteConfirmed(id);
    }
}
