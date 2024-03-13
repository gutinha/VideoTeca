using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using VideoTeca.Controllers;

namespace VideoTeca.Models
{
    public partial class dbContext : DbContext
    {
        public dbContext()
            : base("name=dbContext")
        {
        }

        public virtual DbSet<usuario> usuario { get; set; }
        public virtual DbSet<video> video { get; set; }
        public virtual DbSet<area> area { get; set; }
        public virtual DbSet<subarea> subarea { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<area>()
               .HasMany(e => e.subarea)
               .WithRequired(e => e.area)
               .HasForeignKey(e => e.id_area)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<usuario>()
                .HasMany(e => e.video)
                .WithRequired(e => e.usuario)
                .HasForeignKey(e => e.enviadoPor)
                .WillCascadeOnDelete(false);
        }
    }
}
