namespace Paynau.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Paynau.Infrastructure.Data;

public class PaynauDbContextFactory : IDesignTimeDbContextFactory<PaynauDbContext>
{
    public PaynauDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PaynauDbContext>();

        // 🔹 Usa versión fija de MySQL, no AutoDetect
        optionsBuilder.UseMySql(
            "Server=localhost;Database=PaynauDb;User=root;Password=MySqlP@ssw0rd;",
            ServerVersion.AutoDetect("server=localhost;database=PaynauDb;user=root;password=MySqlP@ssw0rd;")
            // new MySqlServerVersion(new Version(8, 0, 43))
        );

        return new PaynauDbContext(optionsBuilder.Options);
    }
}
