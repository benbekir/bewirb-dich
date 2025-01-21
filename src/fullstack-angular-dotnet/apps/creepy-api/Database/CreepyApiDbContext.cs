using Microsoft.EntityFrameworkCore;

namespace CreepyApi.Database;

public class CreepyApiDbContext : DbContext
{
    public CreepyApiDbContext(DbContextOptions<CreepyApiDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // load all entity configurations in assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CreepyApiDbContext).Assembly);
    }
}
