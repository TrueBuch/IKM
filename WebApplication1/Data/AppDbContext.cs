using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using Route = WebApplication1.Models.Route;

namespace WebApplication1.Data;

/// <summary>
/// Контекст базы данных приложения.
/// Обеспечивает доступ к таблицам базы данных и управляет сущностями.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Набор сущностей водителей.
    /// </summary>
    public DbSet<Driver> Drivers => Set<Driver>();

    /// <summary>
    /// Набор сущностей грузовиков.
    /// </summary>
    public DbSet<Truck> Trucks => Set<Truck>();

    /// <summary>
    /// Набор сущностей грузов.
    /// </summary>
    public DbSet<Cargo> Cargos => Set<Cargo>();

    /// <summary>
    /// Набор сущностей маршрутов.
    /// </summary>
    public DbSet<Route> Routes => Set<Route>();

    /// <summary>
    /// Набор сущностей рейсов.
    /// </summary>
    public DbSet<Trip> Trips => Set<Trip>();

    /// <summary>
    /// Инициализирует новый экземпляр контекста базы данных.
    /// </summary>
    /// <param name="options">Параметры конфигурации контекста.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Конфигурирует модель базы данных при создании контекста.
    /// Используется для дополнительной настройки сущностей и связей.
    /// </summary>
    /// <param name="modelBuilder">Построитель модели.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
