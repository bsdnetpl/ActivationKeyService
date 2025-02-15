using Microsoft.EntityFrameworkCore;

/// <summary>
/// Kontekst bazy danych (Entity Framework)
/// </summary>
public class AppDbContext : DbContext
    {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ActivationKey> ActivationKeys => Set<ActivationKey>();
    }
