using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace _21_910
{
    public partial class MC910DBContext : DbContext
    {
        public MC910DBContext()
            : base("name=MC910DB")
        {
        }

        public virtual DbSet<MC910Table> MC910Table { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
