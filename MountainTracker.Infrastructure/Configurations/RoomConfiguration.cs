using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MountainTracker.Infrastructure.Entities;

namespace MountainTracker.Infrastructure.Configurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.ToTable("Rooms");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(r => r.PasswordHash)
                   .HasMaxLength(200);

            builder.Property(r => r.CreatedAt)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Связи можно настроить Fluent API, если нужно
        }
    }
}
