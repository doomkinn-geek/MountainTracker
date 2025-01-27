using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace MountainTracker.Infrastructure.Data
{
    public class MountainTrackerDbContextFactory : IDesignTimeDbContextFactory<MountainTrackerDbContext>
    {
        public MountainTrackerDbContext CreateDbContext(string[] args)
        {
            // В переменной среды ASPNETCORE_ENVIRONMENT может лежать "Development", "Production" и т.п.
            // Это нужно, если вы используете appsettings.Development.json и т.д.
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            // 1) Создаём ConfigurationBuilder
            //    SetBasePath определяет, откуда мы будем искать файлы .json
            //    Обычно указываем путь к проекту, где лежит ваш appsettings.json (например, WebApi):
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())  // или точнее: Path.Combine(Directory.GetCurrentDirectory(), "..", "MountainTracker.WebApi")
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false) // базовый файл
                .AddJsonFile($"appsettings.{environment}.json", optional: true)         // env-specific
                .AddEnvironmentVariables();

            var configuration = configurationBuilder.Build();

            // 2) Считываем connection string
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // 3) Создаём DbContextOptionsBuilder и настраиваем
            var builder = new DbContextOptionsBuilder<MountainTrackerDbContext>();
            builder.UseNpgsql(connectionString);

            // 4) Возвращаем сам DbContext
            return new MountainTrackerDbContext(builder.Options);
        }
    }
}
