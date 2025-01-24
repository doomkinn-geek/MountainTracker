using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MountainTracker.Infrastructure.Entities;

namespace MountainTracker.Infrastructure.Configurations
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable("Locations");

            builder.HasKey(loc => loc.Id);

            builder.HasOne(loc => loc.User)
                   .WithMany()
                   .HasForeignKey(loc => loc.UserId);

            builder.Property(loc => loc.CreatedAt)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
