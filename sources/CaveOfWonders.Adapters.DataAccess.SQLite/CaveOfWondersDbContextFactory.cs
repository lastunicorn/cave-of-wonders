using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DustInTheWind.CaveOfWonders.Adapters.DataAccess.SQLite;

public class CaveOfWondersDbContextFactory : IDesignTimeDbContextFactory<CaveOfWondersDbContext>
{
    public CaveOfWondersDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<CaveOfWondersDbContext> optionsBuilder = new();
        optionsBuilder.UseSqlite("Data Source=design-time.db");

        return new CaveOfWondersDbContext(optionsBuilder.Options);
    }
}
