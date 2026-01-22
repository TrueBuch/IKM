using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;

/// <summary>
/// Базовый контроллер для CRUD-операций.
/// Предоставляет стандартные действия просмотра, создания,
/// редактирования и удаления сущностей.
/// </summary>
/// <typeparam name="TEntity">Тип сущности, с которой работает контроллер.</typeparam>
public abstract class BaseController<TEntity> : Controller where TEntity : class
{
    /// <summary>
    /// Контекст базы данных приложения.
    /// </summary>
    protected readonly AppDbContext _context;

    /// <summary>
    /// Инициализирует новый экземпляр базового контроллера.
    /// </summary>
    /// <param name="context">Контекст базы данных.</param>
    protected BaseController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Отображает список всех сущностей.
    /// </summary>
    /// <returns>Представление со списком сущностей.</returns>
    public virtual IActionResult Index()
    {
        return View(_context.Set<TEntity>().ToList());
    }

    /// <summary>
    /// Отображает форму создания новой сущности.
    /// </summary>
    /// <returns>Представление создания сущности.</returns>
    public virtual IActionResult Create()
    {
        LoadViewData();
        return View(Activator.CreateInstance<TEntity>());
    }

    /// <summary>
    /// Обрабатывает отправку формы создания новой сущности.
    /// </summary>
    /// <param name="entity">Создаваемая сущность.</param>
    /// <returns>
    /// Перенаправление на список сущностей при успехе
    /// или повторный вывод формы при ошибке валидации.
    /// </returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual IActionResult Create(TEntity entity)
    {
        if (!ModelState.IsValid)
        {
            LoadViewData();
            return View(entity);
        }

        _context.Set<TEntity>().Add(entity);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Отображает форму редактирования сущности.
    /// </summary>
    /// <param name="id">Идентификатор сущности.</param>
    /// <returns>Представление редактирования сущности.</returns>
    public virtual IActionResult Edit(int id)
    {
        var entity = _context.Set<TEntity>().Find(id);
        if (entity == null)
            return NotFound();

        LoadViewData();
        return View(entity);
    }

    /// <summary>
    /// Обрабатывает отправку формы редактирования сущности.
    /// </summary>
    /// <param name="id">Идентификатор сущности.</param>
    /// <param name="entity">Изменённая сущность.</param>
    /// <returns>
    /// Перенаправление на список сущностей при успехе
    /// или повторный вывод формы при ошибке валидации.
    /// </returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual IActionResult Edit(int id, TEntity entity)
    {
        if (!ModelState.IsValid)
        {
            LoadViewData();
            return View(entity);
        }

        _context.Update(entity);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Отображает страницу подтверждения удаления сущности.
    /// </summary>
    /// <param name="id">Идентификатор сущности.</param>
    /// <returns>Представление подтверждения удаления.</returns>
    public virtual IActionResult Delete(int id)
    {
        var entity = _context.Set<TEntity>().Find(id);
        if (entity == null)
            return NotFound();

        return View(entity);
    }

    /// <summary>
    /// Подтверждает удаление сущности.
    /// </summary>
    /// <param name="id">Идентификатор сущности.</param>
    /// <returns>Перенаправление на список сущностей.</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public virtual IActionResult DeleteConfirmed(int id)
    {
        var entity = _context.Set<TEntity>().Find(id);
        if (entity == null)
            return NotFound();

        _context.Set<TEntity>().Remove(entity);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Загружает данные для представлений (ViewBag, SelectList и т.д.).
    /// Может быть переопределён в наследуемых контроллерах.
    /// </summary>
    protected virtual void LoadViewData() { }

    protected virtual bool HasReferences(int id, out string errorMessage)
    {
        errorMessage = string.Empty;
        return false;
    }
}
