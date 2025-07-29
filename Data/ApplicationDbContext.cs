using Microsoft.EntityFrameworkCore;
using WebApplication6.Models;

namespace WebApplication6.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<FicheProjet> FicheProjets { get; set; }
        public DbSet<FicheDetail> FicheDetails { get; set; }
        public DbSet<FicheMetaDonnee> FicheMetaDonnees { get; set; }
        public DbSet<FicheModification> FicheModifications { get; set; }
    }
} 