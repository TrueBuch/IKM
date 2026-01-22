namespace WebApplication1.Models;

/// <summary>
/// Модель главной страницы приложения.
/// Содержит агрегированную статистику по основным сущностям системы
/// управления грузоперевозками.
/// </summary>
public class Home
{
    /// <summary>
    /// Общее количество водителей в системе.
    /// </summary>
    public int DriversCount { get; set; }

    /// <summary>
    /// Общее количество грузовиков в системе.
    /// </summary>
    public int TrucksCount { get; set; }

    /// <summary>
    /// Общее количество грузов в системе.
    /// </summary>
    public int CargosCount { get; set; }

    /// <summary>
    /// Общее количество маршрутов.
    /// </summary>
    public int RoutesCount { get; set; }

    /// <summary>
    /// Общее количество рейсов.
    /// </summary>
    public int TripsCount { get; set; }

    /// <summary>
    /// Количество рейсов, находящихся в процессе выполнения.
    /// </summary>
    public int ActiveTrips { get; set; }

    /// <summary>
    /// Количество водителей, имеющих статус «Свободен».
    /// </summary>
    public int FreeDrivers { get; set; }

    /// <summary>
    /// Количество грузовиков, находящихся в рейсе.
    /// </summary>
    public int BusyTrucks { get; set; }

    /// <summary>
    /// Количество грузов, находящихся в пути.
    /// </summary>
    public int CargosInTransit { get; set; }
}
