using CidadeAtivaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CidadeAtivaApi.Data
{
    public class AppDB(DbContextOptions<AppDB> options) : DbContext(options)
    {
        public DbSet<ProblamasUrbano> Problamas => Set<ProblamasUrbano>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // E-mail único por usuário
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Um usuário pode ter vários chamados
            modelBuilder.Entity<ProblamasUrbano>()
                .HasOne(p => p.User)
                .WithMany(u => u.Problemas)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
