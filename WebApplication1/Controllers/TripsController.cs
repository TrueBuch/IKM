using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Helpers;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

/// <summary>
/// Контроллер для управления рейсами.
/// Реализует просмотр, создание, редактирование и удаление рейсов,
/// а также бизнес-логику, связанную с автоматическим управлением
/// статусами водителей, грузовиков и грузов.
/// </summary>
public class TripsController : BaseController<Trip>
{
    /// <summary>
    /// Инициализирует новый экземпляр контроллера рейсов.
    /// </summary>
    /// <param name="context">Контекст базы данных приложения.</param>
    public TripsController(AppDbContext context) : base(context) { }

    /// <summary>
    /// Отображает список всех рейсов с подгруженными связанными сущностями
    /// (водитель, грузовик, груз, маршрут).
    /// </summary>
    /// <returns>Представление со списком рейсов.</returns>
    public override IActionResult Index()
    {
        var trips = _context.Trips
            .Include(t => t.Driver)
            .Include(t => t.Truck)
            .Include(t => t.Cargo)
            .Include(t => t.Route)
            .ToList();

        return View(trips);
    }

    /// <summary>
    /// Отображает форму создания нового рейса.
    /// </summary>
    /// <returns>Представление создания рейса.</returns>
    public override IActionResult Create()
    {
        LoadViewData(null);
        return View();
    }

    /// <summary>
    /// Отображает форму редактирования существующего рейса.
    /// </summary>
    /// <param name="id">Идентификатор редактируемого рейса.</param>
    /// <returns>Представление редактирования рейса.</returns>
    public override IActionResult Edit(int id)
    {
        var trip = _context.Trips.Find(id);
        if (trip == null)
            return NotFound();

        LoadViewData(trip);
        return View(trip);
    }

    /// <summary>
    /// Обрабатывает создание нового рейса.
    /// Выполняет перерасчёт статуса рейса, проверку конфликтов
    /// и обновление связанных сущностей.
    /// </summary>
    /// <param name="trip">Создаваемый рейс.</param>
    /// <returns>Редирект на список рейсов или форма с ошибками.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public override IActionResult Create(Trip trip)
    {
        trip.RecalculateStatus();
        ValidateActiveTripConflicts(trip);

        if (!ModelState.IsValid)
        {
            LoadViewData(trip);
            return View(trip);
        }

        ApplyTripSideEffects(trip);

        _context.Trips.Add(trip);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Обрабатывает редактирование существующего рейса.
    /// </summary>
    /// <param name="id">Идентификатор рейса.</param>
    /// <param name="trip">Изменённые данные рейса.</param>
    /// <returns>Редирект на список рейсов или форма с ошибками.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public override IActionResult Edit(int id, Trip trip)
    {
        if (id != trip.Id)
            return BadRequest();

        trip.RecalculateStatus();
        ValidateActiveTripConflicts(trip, id);

        if (!ModelState.IsValid)
        {
            LoadViewData(trip);
            return View(trip);
        }

        ApplyTripSideEffects(trip);

        _context.Update(trip);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Загружает данные для выпадающих списков
    /// без учёта текущего рейса.
    /// </summary>
    protected override void LoadViewData()
    {
        LoadViewData(null);
    }

    /// <summary>
    /// Загружает данные для выпадающих списков формы
    /// с учётом редактируемого рейса.
    /// </summary>
    /// <param name="trip">
    /// Редактируемый рейс или <c>null</c> при создании нового.
    /// </param>
    private void LoadViewData(Trip? trip)
    {
        var allowedDriverStatuses = new[]
        {
            DriverStatuses.Free.GetPgName(),
            DriverStatuses.Trip.GetPgName()
        };

        var driversQuery = _context.Drivers.AsQueryable();

        driversQuery = trip == null
            ? driversQuery.Where(d => allowedDriverStatuses.Contains(d.DriverStatus.ToString()))
            : driversQuery.Where(d =>
                d.Id == trip.DriverId ||
                allowedDriverStatuses.Contains(d.DriverStatus.ToString()));

        ViewBag.Drivers = new SelectList(
            driversQuery.ToList(),
            "Id",
            "FullName",
            trip?.DriverId
        );

        var allowedTruckStatuses = new[]
        {
            TruckStatuses.Free.GetPgName(),
            TruckStatuses.InTrip.GetPgName()
        };

        var trucksQuery = _context.Trucks.AsQueryable();

        trucksQuery = trip == null
            ? trucksQuery.Where(t => allowedTruckStatuses.Contains(t.TruckStatus.ToString()))
            : trucksQuery.Where(t =>
                t.Id == trip.TruckId ||
                allowedTruckStatuses.Contains(t.TruckStatus.ToString()));

        ViewBag.Trucks = new SelectList(
            trucksQuery.ToList(),
            "Id",
            "PlateNumber",
            trip?.TruckId
        );

        var usedCargoIds = _context.Trips
            .Select(t => t.CargoId)
            .ToHashSet();

        var cargosQuery = _context.Cargos.AsQueryable();

        cargosQuery = trip == null
            ? cargosQuery.Where(c => !usedCargoIds.Contains(c.Id))
            : cargosQuery.Where(c =>
                c.Id == trip.CargoId ||
                !usedCargoIds.Contains(c.Id));

        ViewBag.Cargos = new SelectList(
            cargosQuery.ToList(),
            "Id",
            "Description",
            trip?.CargoId
        );

        ViewBag.Routes = new SelectList(
            _context.Routes,
            "Id",
            "FullRoute",
            trip?.RouteId
        );
    }

    /// <summary>
    /// Применяет побочные эффекты рейса:
    /// обновляет статусы водителя, грузовика и груза
    /// в зависимости от текущего статуса рейса.
    /// </summary>
    /// <param name="trip">Рейс, для которого применяются изменения.</param>
    private void ApplyTripSideEffects(Trip trip)
    {
        trip.RecalculateStatus();

        var driver = _context.Drivers.Find(trip.DriverId);
        var truck  = _context.Trucks.Find(trip.TruckId);
        var cargo  = _context.Cargos.Find(trip.CargoId);

        if (driver == null || truck == null || cargo == null)
            return;

        switch (trip.TripStatus)
        {
            case TripStatuses.Planned:
                driver.DriverStatus = DriverStatuses.Free;
                truck.TruckStatus   = TruckStatuses.Free;
                cargo.CargoStatus   = CargoStatuses.NotDelivered;
                break;

            case TripStatuses.InProgress:
                driver.DriverStatus = DriverStatuses.Trip;
                truck.TruckStatus   = TruckStatuses.InTrip;
                cargo.CargoStatus   = CargoStatuses.InTransit;
                break;

            case TripStatuses.Completed:
                driver.DriverStatus = DriverStatuses.Free;
                truck.TruckStatus   = TruckStatuses.Free;
                cargo.CargoStatus   = CargoStatuses.Delivered;
                break;
        }
    }

    /// <summary>
    /// Проверяет наличие конфликтов активных рейсов:
    /// запрещает наличие более одного выполняющегося рейса
    /// у одного водителя или грузовика.
    /// </summary>
    /// <param name="trip">Проверяемый рейс.</param>
    /// <param name="tripId">
    /// Идентификатор редактируемого рейса
    /// (используется для исключения самого себя).
    /// </param>
    private void ValidateActiveTripConflicts(Trip trip, int? tripId = null)
    {
        if (trip.TripStatus != TripStatuses.InProgress)
            return;

        var inProgressPg = TripStatuses.InProgress.GetPgName();

        bool driverBusy = _context.Trips.Any(t =>
            t.DriverId == trip.DriverId &&
            t.TripStatus.ToString() == inProgressPg &&
            (tripId == null || t.Id != tripId)
        );

        if (driverBusy)
        {
            ModelState.AddModelError(
                nameof(Trip.DriverId),
                "У выбранного водителя уже есть выполняющийся рейс"
            );
        }

        bool truckBusy = _context.Trips.Any(t =>
            t.TruckId == trip.TruckId &&
            t.TripStatus.ToString() == inProgressPg &&
            (tripId == null || t.Id != tripId)
        );

        if (truckBusy)
        {
            ModelState.AddModelError(
                nameof(Trip.TruckId),
                "Выбранный грузовик уже находится в рейсе"
            );
        }
    }
}
