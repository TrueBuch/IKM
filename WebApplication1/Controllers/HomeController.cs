using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Helpers;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

/// <summary>
/// Главный контроллер приложения.
/// Отвечает за отображение главной страницы, общей статистики
/// и пересчёт статусов сущностей на основе текущих рейсов.
/// </summary>
public class HomeController : Controller
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Инициализирует новый экземпляр главного контроллера.
    /// </summary>
    /// <param name="context">Контекст базы данных приложения.</param>
    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Отображает главную страницу приложения.
    /// Формирует сводную информацию о количестве записей
    /// в основных таблицах базы данных.
    /// </summary>
    /// <returns>Представление главной страницы.</returns>
    public IActionResult Index()
    {
        var inProgressTripPg = TripStatuses.InProgress.GetPgName();
        var freeDriverPg     = DriverStatuses.Free.GetPgName();
        var inTripTruckPg    = TruckStatuses.InTrip.GetPgName();
        var cargoInTransitPg = CargoStatuses.InTransit.GetPgName();

        var model = new Home
        {
            DriversCount = _context.Drivers.Count(),
            TrucksCount  = _context.Trucks.Count(),
            CargosCount  = _context.Cargos.Count(),
            RoutesCount  = _context.Routes.Count(),
            TripsCount   = _context.Trips.Count(),

            ActiveTrips = _context.Trips
                .Count(t => t.TripStatus.ToString() == inProgressTripPg),

            FreeDrivers = _context.Drivers
                .Count(d => d.DriverStatus.ToString() == freeDriverPg),

            BusyTrucks = _context.Trucks
                .Count(t => t.TruckStatus.ToString() == inTripTruckPg),

            CargosInTransit = _context.Cargos
                .Count(c => c.CargoStatus.ToString() == cargoInTransitPg)
        };

        return View(model);
    }


    /// <summary>
    /// Запускает пересчёт всех статусов в системе.
    /// Используется для синхронизации статусов водителей,
    /// грузовиков и грузов на основе текущего состояния рейсов.
    /// </summary>
    /// <returns>Перенаправление на главную страницу.</returns>
    public IActionResult RefreshStatuses()
    {
        RecalculateAllStatuses();
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Отображает страницу с информацией о конфиденциальности.
    /// </summary>
    /// <returns>Представление страницы Privacy.</returns>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Выполняет полный пересчёт статусов рейсов, водителей,
    /// грузовиков и грузов.
    /// </summary>
    private void RecalculateAllStatuses()
    {
        var trips = _context.Trips.ToList();

        foreach (var trip in trips)
        {
            trip.RecalculateStatus();
        }

        ResetAllStatuses();
        ApplyActiveTrips();

        _context.SaveChanges();
    }

    /// <summary>
    /// Сбрасывает статусы всех водителей, грузовиков и грузов
    /// в значения по умолчанию перед повторным пересчётом.
    /// </summary>
    private void ResetAllStatuses()
    {
        foreach (var driver in _context.Drivers)
            driver.DriverStatus = DriverStatuses.Free;

        foreach (var truck in _context.Trucks)
            truck.TruckStatus = TruckStatuses.Free;

        foreach (var cargo in _context.Cargos)
            cargo.CargoStatus = CargoStatuses.NotDelivered;
    }

    /// <summary>
    /// Применяет статусы активных и завершённых рейсов
    /// к связанным водителям, грузовикам и грузам.
    /// </summary>
    private void ApplyActiveTrips()
    {
        var trips = _context.Trips.ToList();

        foreach (var driver in _context.Drivers)
        {
            if (trips.Any(t =>
                t.DriverId == driver.Id &&
                t.TripStatus == TripStatuses.InProgress))
            {
                driver.DriverStatus = DriverStatuses.Trip;
            }
        }

        foreach (var truck in _context.Trucks)
        {
            if (trips.Any(t =>
                t.TruckId == truck.Id &&
                t.TripStatus == TripStatuses.InProgress))
            {
                truck.TruckStatus = TruckStatuses.InTrip;
            }
        }

        foreach (var cargo in _context.Cargos)
        {
            if (trips.Any(t =>
                t.CargoId == cargo.Id &&
                t.TripStatus == TripStatuses.InProgress))
            {
                cargo.CargoStatus = CargoStatuses.InTransit;
            }
            else if (trips.Any(t =>
                t.CargoId == cargo.Id &&
                t.TripStatus == TripStatuses.Completed))
            {
                cargo.CargoStatus = CargoStatuses.Delivered;
            }
        }
    }
}
