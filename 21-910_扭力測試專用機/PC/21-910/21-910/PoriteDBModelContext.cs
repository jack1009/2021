using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace _21_910
{
    public partial class PoriteDBModelContext : DbContext
    {
        public PoriteDBModelContext()
            : base("name=PoriteDBModel")
        {
        }

        public virtual DbSet<MC908Table> MC908Table { get; set; }
        public virtual DbSet<MC909Table> MC909Table { get; set; }
        public virtual DbSet<MC910Table> MC910Table { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MC908Table>()
                .Property(e => e.QRCode)
                .IsFixedLength();

            modelBuilder.Entity<MC909Table>()
                .Property(e => e.QRCode)
                .IsFixedLength();

            modelBuilder.Entity<MC910Table>()
                .Property(e => e.QRCode)
                .IsFixedLength();
        }
    }
}
