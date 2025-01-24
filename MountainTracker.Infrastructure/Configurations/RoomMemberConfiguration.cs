using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MountainTracker.Infrastructure.Entities;

namespace MountainTracker.Infrastructure.Configurations
{
    public class RoomMemberConfiguration : IEntityTypeConfiguration<RoomMember>
    {
        public void Configure(EntityTypeBuilder<RoomMember> builder)
        {
            builder.ToTable("RoomMembers");

            builder.HasKey(rm => new { rm.RoomId, rm.UserId });

            builder.Property(rm => rm.RoleInRoom)
                   .HasMaxLength(20)
                   .HasDefaultValue("user");

            builder.Property(rm => rm.JoinedAt)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(rm => rm.Room)
                   .WithMany(r => r.RoomMembers)
                   .HasForeignKey(rm => rm.RoomId);

            builder.HasOne(rm => rm.User)
                   .WithMany(u => u.RoomMembers)
                   .HasForeignKey(rm => rm.UserId);
        }
    }
}
