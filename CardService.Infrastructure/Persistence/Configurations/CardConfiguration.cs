using CardService.Application.Common.Helpers;
using CardService.Domain.Entities;
using CardService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CardService.Infrastructure.Persistence.Configurations
{
    public class CardConfiguration : IEntityTypeConfiguration<CardEntity>
    {
        public void Configure(EntityTypeBuilder<CardEntity> builder)
        {
            builder.ToTable(BaseStrings.CARD_TBL);
            builder.Property(p => p.Name)
                .HasMaxLength(128)
                .IsRequired(true);

            builder.Property(p => p.Description)
                .HasMaxLength(256);

            builder.Property(p => p.Color)
              .HasMaxLength(256);

            builder.Property(p => p.Status)
                .HasMaxLength(64)
                .HasDefaultValue(Status.ToDo)
                .HasConversion<string>();

            builder.HasOne(e => e.User)
              .WithMany()
              .HasForeignKey(e => e.UserId);

            builder.Property(p => p.DateCreated)
                .HasMaxLength(40);

            builder.Property(p => p.DateUpdated)
                .HasMaxLength(40);

            builder.HasIndex(i => new { i.Name }).IsUnique();
        }
    }
}
