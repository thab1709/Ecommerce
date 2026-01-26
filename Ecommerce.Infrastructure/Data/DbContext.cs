
using System.Text.Json;

namespace Ecommerce.Infrastructure.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options ) : base(options) {}
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Customer> Customers =>Set<Customer>();
    public DbSet<Auditlog> auditlogs {get; set;}
    public DbSet<Promotion> Promotions { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Order>()
         .HasQueryFilter(o => !o.Deleted);
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
        .Where(e =>e.State ==EntityState.Modified || e.State == EntityState.Deleted);
        foreach (var entry in entries)
        {
            var log = new Auditlog
            {
               EntityName = entry.Entity.GetType().Name,
               EntityId = entry.Property("Id").CurrentValue?.ToString(),
               Action = entry.State.ToString(),
               OldValue  =  JsonSerializer.Serialize(entry.OriginalValues.ToObject()),
               NewValue = entry.State == EntityState.Modified 
                        ? JsonSerializer.Serialize(entry.CurrentValues.ToObject()) 
                        : null

            };
            auditlogs.Add(log);
        }

         return await base.SaveChangesAsync(cancellationToken);

    }



}