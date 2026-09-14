using DWES_01_Agenda.Models;
using Microsoft.EntityFrameworkCore;

namespace DWES_01_Agenda.Entity;

public class AppDbContext : DbContext
{
    public DbSet<ContactoEntity> Agenda { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=Agenda.db");
        }
    }
}