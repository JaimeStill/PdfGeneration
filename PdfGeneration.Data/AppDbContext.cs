using Microsoft.EntityFrameworkCore;
using PdfGeneration.Data.Entities;
using System.Linq;

namespace PdfGeneration.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Person> People { get; set; }
        public DbSet<PersonAssociate> Associates { get; set; }
        public DbSet<Upload> Uploads { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Model
                .GetEntityTypes()
                .ToList()
                .ForEach(x =>
                {
                    modelBuilder
                        .Entity(x.Name)
                        .ToTable(x.Name.Split('.').Last());
                });
                
            modelBuilder.Entity<PersonAssociate>()
                .HasOne(x => x.Person)
                .WithMany(x => x.Associates)
                .HasForeignKey(x => x.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PersonAssociate>()
                .HasOne(x => x.Associate)
                .WithMany(x => x.People)
                .HasForeignKey(x => x.AssociateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
