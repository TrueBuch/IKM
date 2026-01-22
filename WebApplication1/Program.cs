using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
// using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
            npgsqlOptions.MapEnum<DriverStatuses>("truck_drivers.driver_statuses");
            npgsqlOptions.MapEnum<TruckStatuses>("truck_drivers.truck_statuses");
            npgsqlOptions.MapEnum<CargoStatuses>("truck_drivers.cargo_statuses");
            npgsqlOptions.MapEnum<CargoTypes>("truck_drivers.cargo_types");
            npgsqlOptions.MapEnum<TripStatuses>("truck_drivers.trip_statuses");
        }
    )
);

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();