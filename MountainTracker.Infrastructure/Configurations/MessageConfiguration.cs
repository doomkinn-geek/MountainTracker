using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MountainTracker.Infrastructure.Entities;

namespace MountainTracker.Infrastructure.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages");

            builder.HasKey(m => m.Id);

            builder.HasOne(m => m.Room)
                   .WithMany(r => r.Messages)
                   .HasForeignKey(m => m.RoomId);

            builder.HasOne(m => m.Author)
                   .WithMany()
                   .HasForeignKey(m => m.AuthorId);

            builder.Property(m => m.Text)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(m => m.CreatedAt)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
