using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SmartHiring.Core.Entities;

namespace SmartHiring.Repository.Data.Configurations
{
    public class NoteConfig : IEntityTypeConfiguration<Note>
    {
        public void Configure(EntityTypeBuilder<Note> builder)
        {
            builder.ToTable("Notes");

            builder.HasKey(n => n.Id);
            builder.Property(n => n.Header)
                .IsRequired();

            builder.Property(n => n.Content)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(n => n.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.HasOne(n => n.User)
                .WithMany(u => u.Notes)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(n => n.Post)
                .WithMany(u => u.Notes)
                .HasForeignKey(n => n.PostId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
