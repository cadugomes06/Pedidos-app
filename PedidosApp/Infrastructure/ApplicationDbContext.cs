using Microsoft.EntityFrameworkCore;
using PedidosApp.Domain;

namespace PedidosApp.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Valor).HasPrecision(10, 2);
            });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Order> Orders
        {
            get; set;
        }


    }
}
