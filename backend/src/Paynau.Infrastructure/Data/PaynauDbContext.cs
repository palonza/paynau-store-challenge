
namespace Paynau.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Paynau.Domain.Entities;

public class PaynauDbContext : DbContext
{
    public PaynauDbContext(DbContextOptions<PaynauDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaynauDbContext).Assembly);
    }
}

