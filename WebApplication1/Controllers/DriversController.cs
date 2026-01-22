using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Helpers;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

/// <summary>
/// Контроллер для управления водителями.
/// Обеспечивает просмотр, добавление, редактирование и удаление водителей,
/// а также работу со статусами водителей.
/// </summary>
public class DriversController : BaseController<Driver>
{
    /// <summary>
    /// Инициализирует новый экземпляр контроллера водителей.
    /// </summary>
    /// <param name="context">Контекст базы данных приложения.</param>
    public DriversController(AppDbContext context) : base(context) { }

    /// <summary>
    /// Загружает данные, необходимые для представлений водителей.
    /// Формирует списки доступных статусов водителей.
    /// </summary>
    protected override void LoadViewData()
    {
        ViewBag.EditableStatuses = EnumHelper.FromEnumExcept(DriverStatuses.Trip);
        ViewBag.Statuses = EnumHelper.FromEnum<DriverStatuses>();
    }

    /// <summary>
    /// Отображает список водителей.
    /// Свободные водители отображаются первыми, затем сортировка выполняется по фамилии.
    /// </summary>
    /// <returns>Представление со списком водителей.</returns>
    public override IActionResult Index()
    {
        var freePgValue = DriverStatuses.Free.GetPgName();

        var drivers = _context.Drivers
            .OrderByDescending(d => d.DriverStatus.ToString() == freePgValue)
            .ThenBy(d => d.Surname)
            .ToList();

        return View(drivers);
    }

    /// <summary>
    /// Подтверждает удаление водителя.
    /// Удаление запрещено, если водитель участвует хотя бы в одном рейсе.
    /// </summary>
    /// <param name="id">Идентификатор водителя.</param>
    /// <returns>
    /// Представление подтверждения удаления с ошибкой
    /// либо перенаправление на список водителей.
    /// </returns>
    public override IActionResult DeleteConfirmed(int id)
    {
        bool hasTrips = _context.Trips.Any(t => t.DriverId == id);

        if (hasTrips)
        {
            ModelState.AddModelError(string.Empty,
                "Нельзя удалить водителя, у которого есть связанные рейсы.");

            var driver = _context.Drivers.Find(id);
            return View("Delete", driver);
        }

        return base.DeleteConfirmed(id);
    }
}
