using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EventService.Entities;

namespace EventService.Data
{
    public class EventServiceContext : DbContext
    {
        public EventServiceContext(DbContextOptions<EventServiceContext> options)
            : base(options)
        {
        }

        public DbSet<GroupModel> Groups { get; set; }
        public DbSet<EventModel> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurer la liste des identifiants des managers et abonnés
            modelBuilder.Entity<GroupModel>()
                .Property(g => g.ManagerIds)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList());

            modelBuilder.Entity<GroupModel>()
                .Property(g => g.SubscriberIds)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList());

            // Configurer la relation entre EventModel et GroupModel avec une clé étrangère
            modelBuilder.Entity<EventModel>()
                .HasOne<GroupModel>() // Relation avec GroupModel
                .WithMany() // Un groupe peut avoir plusieurs événements
                .HasForeignKey(e => e.GroupId) // Utilise GroupId comme clé étrangère
                .OnDelete(DeleteBehavior.Cascade); // Supprime les événements liés si le groupe est supprimé
        }
    }
}