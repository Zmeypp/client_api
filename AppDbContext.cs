using ClientsAPI;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Client> Clients { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
