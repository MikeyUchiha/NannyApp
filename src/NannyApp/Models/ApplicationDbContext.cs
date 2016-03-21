using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;

namespace NannyApp.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<FilePath> FilePaths { get; set; }
        public DbSet<Family> Families { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<Activity> Activities { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<FilePath>()
                .HasOne(p => p.User)
                .WithOne(i => i.ProfilePhoto)
                .HasForeignKey<FilePath>(p => p.UserId);

            builder.Entity<Connection>()
                .HasOne(c => c.User)
                .WithMany(u => u.Connections)
                .HasForeignKey(c => c.UserId);

            builder.Entity<Connection>()
                .HasOne(c => c.Family)
                .WithMany(f => f.Connections)
                .HasForeignKey(c => c.FamilyId);

            builder.Entity<Activity>()
                .Property(a => a.Group)
                .IsRequired();

            builder.Entity<Activity>()
                .Property(a => a.Category)
                .IsRequired();
        }
    }
}
