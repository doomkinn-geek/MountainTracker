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

            // 1) ������ ������ ����������� �� ������������
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // 2) ������������ DbContext � ��������� ���������� (Npgsql, SqlServer, � �.�.)
            builder.Services.AddDbContext<MountainTrackerDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
                // ��� options.UseSqlServer(connectionString);
            });

            // 3) ��������� �����������
            builder.Services.AddControllers();

            // 4) ��������� Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MountainTracker API",
                    Version = "v1",
                    Description = "API ��� ������������ ����������, ����, ����������� � �.�.",
                });

                // ���� �� ����������� JWT, ����� ��������� ����� ������������ � Swagger:
                // var securityScheme = new OpenApiSecurityScheme
                // {
                //     Name = "Authorization",
                //     Type = SecuritySchemeType.Http,
                //     Scheme = "bearer",
                //     BearerFormat = "JWT",
                //     In = ParameterLocation.Header,
                //     Description = "������� JWT �����"
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

            // 5) (�����������) ����������� �������������� (JWT)
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

            // 6) ������������ ����������� (��� �������) �� Infrastructure
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRoomRepository, RoomRepository>();
            builder.Services.AddScoped<ILocationRepository, LocationRepository>();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddScoped<IReminderRepository, ReminderRepository>();
            // ���� ����������� GenericRepo:
            // builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // 7) ������ ����������
            var app = builder.Build();

            // 8) (�����������) ��������� �������� ��� ������
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MountainTrackerDbContext>();
                // ����� �������� (���� �� ����������� Migrations):
                dbContext.Database.Migrate();
                // ��� dbContext.Database.EnsureCreated(); (�� ��� �� ������������� ������ � ����������)
            }

            // 9) Middleware-��������: Swagger, HTTPS, Auth, Controllers � ��.

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MountainTracker API v1");
                });
            }

            app.UseHttpsRedirection();

            // (�����������) ���� ���� ��������� ��������������:
            // app.UseAuthentication();

            app.UseAuthorization();

            // 10) ���������� �����������
            app.MapControllers();

            // 11) ��������� ����������
            app.Run();
        }
    }
}
