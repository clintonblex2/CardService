using CardService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CardService.Infrastructure.Persistence.DatabaseContext
{
    public class CardDbContext : DbContext
    {
        public CardDbContext(DbContextOptions<CardDbContext> options)
            : base(options) { }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<CardEntity> Cards { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
