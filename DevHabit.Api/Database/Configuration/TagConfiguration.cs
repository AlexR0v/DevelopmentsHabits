using DevHabit.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevHabit.Api.Database.Configuration;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(500);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(100);

        builder.HasIndex(x => new { x.Name }).IsUnique();
    }
}
