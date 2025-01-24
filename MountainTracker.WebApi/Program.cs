using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MountainTracker.Infrastructure.Data;
using MountainTracker.Infrastructure.Repositories.Implementations;
using MountainTracker.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MountainTracker.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1) Читаем строку подключения из конфигурации
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // 2) Регистрируем DbContext с указанием провайдера (Npgsql, SqlServer, и т.д.)
            builder.Services.AddDbContext<MountainTrackerDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
                // или options.UseSqlServer(connectionString);
            });

            // 3) Добавляем контроллеры
            builder.Services.AddControllers();

            // 4) Настройка Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MountainTracker API",
                    Version = "v1",
                    Description = "API для отслеживания геолокации, чата, напоминаний и т.д.",
                });

                // Если вы используете JWT, можно настроить схему безопасности в Swagger:
                // var securityScheme = new OpenApiSecurityScheme
                // {
                //     Name = "Authorization",
                //     Type = SecuritySchemeType.Http,
                //     Scheme = "bearer",
                //     BearerFormat = "JWT",
                //     In = ParameterLocation.Header,
                //     Description = "Введите JWT токен"
                // };
                //
                // c.AddSecurityDefinition("Bearer", securityScheme);
                // c.AddSecurityRequirement(new OpenApiSecurityRequirement
                // {
                //     {
                //         new OpenApiSecurityScheme
                //         {
                //             Reference = new OpenApiReference
                //             {
                //                 Type = ReferenceType.SecurityScheme,
                //                 Id = "Bearer"
                //             }
                //         },
                //         new string[] {}
                //     }
                // });
            });

            // 5) (Опционально) Настраиваем аутентификацию (JWT)
            // builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //     .AddJwtBearer(options =>
            //     {
            //         options.TokenValidationParameters = new TokenValidationParameters
            //         {
            //             ValidateIssuer = true,
            //             ValidateAudience = true,
            //             ValidateLifetime = true,
            //             ValidateIssuerSigningKey = true,
            //             ValidIssuer = builder.Configuration["Jwt:Issuer"],
            //             ValidAudience = builder.Configuration["Jwt:Audience"],
            //             IssuerSigningKey = new SymmetricSecurityKey(
            //                 Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
            //         };
            //     });

            // 6) Регистрируем репозитории (или сервисы) из Infrastructure
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRoomRepository, RoomRepository>();
            builder.Services.AddScoped<ILocationRepository, LocationRepository>();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddScoped<IReminderRepository, ReminderRepository>();
            // Если используете GenericRepo:
            // builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // 7) Создаём приложение
            var app = builder.Build();

            // 8) (Опционально) Применяем миграции при старте
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MountainTrackerDbContext>();
                // Вызов миграции (если вы используете Migrations):
                dbContext.Database.Migrate();
                // или dbContext.Database.EnsureCreated(); (но это не рекомендуется вместе с миграциями)
            }

            // 9) Middleware-пайплайн: Swagger, HTTPS, Auth, Controllers и пр.

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MountainTracker API v1");
                });
            }

            app.UseHttpsRedirection();

            // (Опционально) Если была настроена аутентификация:
            // app.UseAuthentication();

            app.UseAuthorization();

            // 10) Подключаем контроллеры
            app.MapControllers();

            // 11) Запускаем приложение
            app.Run();
        }
    }
}
