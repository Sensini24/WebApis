using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using DemoMinimalAPI.Data;
using Microsoft.AspNetCore.Hosting;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private SqliteConnection _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {


        // 1. Crear una nueva base SQLite en memoria
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        builder.ConfigureServices(services =>
        {
            // 2. Eliminar el DataContext original
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<DataContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            // 3. Agregar nuestro contexto con esa conexión de sqlite en memoria
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            // 4. Construir el servicio y aplicar migraciones
            var provider = services.BuildServiceProvider();

            using (var scope = provider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                db.Database.EnsureCreated(); // crea las tablas en memoria
            }
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection?.Dispose();
    }
}
