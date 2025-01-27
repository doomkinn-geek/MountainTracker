﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MountainTracker.Infrastructure.Entities;

namespace MountainTracker.Infrastructure.Data
{
    public class MountainTrackerDbContext
        : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public MountainTrackerDbContext(DbContextOptions<MountainTrackerDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<Room> Rooms { get; set; } = null!;
        public DbSet<RoomMember> RoomMembers { get; set; } = null!;
        public DbSet<Location> Locations { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<Reminder> Reminders { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Применяем все конфигурации IEntityTypeConfiguration<T>, 
            // найденные в сборке (в папке Configurations)
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MountainTrackerDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
