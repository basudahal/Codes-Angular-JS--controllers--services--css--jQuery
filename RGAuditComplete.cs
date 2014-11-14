namespace RGAuditTool.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class RGAuditComplete : DbContext
    {
        public RGAuditComplete()
            : base("name=RGAuditComplete")
        {
        }


        //public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditStatu>()
                .Property(e => e.Extension)
                .IsFixedLength();

            modelBuilder.Entity<AuditStatu>()
                .HasMany(e => e.PerformanceAudits)
                .WithRequired(e => e.AuditStatu)
                .HasForeignKey(e => e.AuditNumber)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserRole>()
                .Property(e => e.RoleName)
                .IsFixedLength();
        }
    }
}
