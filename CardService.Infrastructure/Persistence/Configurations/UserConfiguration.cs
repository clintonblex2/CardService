using CardService.Application.Common.Helpers;
using CardService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CardService.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.ToTable(BaseStrings.USER_TBL);
            builder.Property(p => p.Email)
                .HasMaxLength(128);

            builder.Property(p => p.Password)
                .HasMaxLength(256);

            builder.Property(e => e.Role)
                .HasMaxLength(64)
                .HasConversion<string>();

            builder.Property(p => p.DateCreated)
                .HasMaxLength(40);

            builder.Property(p => p.DateUpdated)
                .HasMaxLength(40);

            builder.HasIndex(i => new { i.Email, i.Role }).IsUnique();
        }
    }
}
