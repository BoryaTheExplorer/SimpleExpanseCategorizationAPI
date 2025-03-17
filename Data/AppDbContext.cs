using ExpanseCategorizationAPI.Models;
using Microsoft.EntityFrameworkCore;



namespace ExpanseCategorizationAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Transaction> Transactions { get; set; } = null!;
    }
}