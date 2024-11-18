using Microsoft.EntityFrameworkCore;
    
public class AppDbContext : DbContext
{
    private readonly TenantProvider _tenantProvider;
    public AppDbContext(DbContextOptions<AppDbContext> options, TenantProvider tenantProvider)
        : base(options)
    {   
        _tenantProvider = tenantProvider;
    }

    public virtual DbSet<CatalogEntity> Catalogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CatalogEntity>()
            .HasQueryFilter(y => y.TenantId == _tenantProvider.TenantId);
    }
}