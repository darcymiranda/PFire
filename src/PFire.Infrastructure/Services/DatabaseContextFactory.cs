using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PFire.Infrastructure.Services
{
    /// <summary>
    ///     Used to generate a <see cref="DatabaseContext" /> for creating migrations.
    /// </summary>
    // ReSharper disable once UnusedType.Global
    internal class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseSqlite("Data Source=pfiredb.sqlite");

            return new DatabaseContext(optionsBuilder.Options);
        }
    }
}
