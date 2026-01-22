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
}
