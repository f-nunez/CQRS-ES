using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CQRSES.Command.Infrastructure.Persistence.EntityFrameworkCore;

public class StreamStateConfiguration : IEntityTypeConfiguration<StreamState>
{
    public void Configure(EntityTypeBuilder<StreamState> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(s => s.CreatedOn)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(s => s.EventPayload)
            .IsRequired();

        builder.Property(s => s.EventType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.StreamName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.Version)
            .IsRequired();

        builder.HasIndex(s => new { s.Version, s.StreamName });
    }
}