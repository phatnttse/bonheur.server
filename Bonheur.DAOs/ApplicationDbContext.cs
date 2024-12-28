using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Bonheur.BusinessObjects.Entities;
using Bonheur.BusinessObjects.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bonheur.DAOs
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplierCategory> SupplierCategories { get; set; }
        public DbSet<SupplierImage> SupplierImages { get; set; }
        public DbSet<RequestPricing> RequestPricings { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<SubscriptionPackage> SubscriptionPackages { get; set; }
        public DbSet<Review> Reviews { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure ApplicationUser and ApplicationRole relationships with Identity tables
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Roles)
                .WithOne()
                .HasForeignKey(ur => ur.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<ApplicationUser>()
               .HasMany(u => u.Claims)
               .WithOne()
               .HasForeignKey(c => c.UserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Gender)
                .HasConversion(new EnumToStringConverter<Gender>());

            modelBuilder.Entity<ApplicationRole>()
                .HasMany(r => r.Users)
                .WithOne()
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationRole>()
                .HasMany(r => r.Claims)
                .WithOne()
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Supplier>()
                .HasOne(s => s.User)
                .WithMany()  // Assuming one-to-many relationship with ApplicationUser
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.SetNull); // Adjust delete behavior as needed

            // Configure Supplier -> SupplierCategory (many-to-one relationship)
            modelBuilder.Entity<Supplier>()
                .HasOne(s => s.SupplierCategory)
                .WithMany()
                .HasForeignKey(s => s.SupplierCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Supplier>()
                .Property(u => u.Status)
                .HasConversion(new EnumToStringConverter<SupplierStatus>());


            modelBuilder.Entity<Supplier>()
                .Property(u => u.OnBoardStatus)
                .HasConversion(new EnumToStringConverter<OnBoardStatus>());

            // Configure SupplierImage -> Supplier (many-to-one relationship)
            modelBuilder.Entity<SupplierImage>()
                .HasOne(si => si.Supplier)
                .WithMany(s => s.SupplierImages)
                .HasForeignKey(si => si.SupplierId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure RequestPricing -> Supplier (many-to-one relationship)
            modelBuilder.Entity<RequestPricing>()
                .HasOne(rp => rp.Supplier)
                .WithMany()
                .HasForeignKey(rp => rp.SupplierId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RequestPricing>()
                .Property(rp => rp.Status)
                .HasConversion<string>();

            // Configure Review -> Supplier (many-to-one relationship)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Supplier)
                .WithMany()
                .HasForeignKey(r => r.SupplierId);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany() 
                .HasForeignKey(r => r.UserId);


            // Configure Advertisement -> Supplier (many-to-one relationship)
            modelBuilder.Entity<Advertisement>()
                .HasOne(a => a.Supplier)
                .WithMany(s => s.Advertisements)
                .HasForeignKey(a => a.SupplierId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Supplier -> SubscriptionPackage (optional one-to-one or many-to-one relationship)
            modelBuilder.Entity<Supplier>()
                .HasOne(s => s.SubscriptionPackage)
                .WithMany() // Assuming one-to-many relationship with SubscriptionPackage
                .HasForeignKey(s => s.SubscriptionPackageId)
                .OnDelete(DeleteBehavior.SetNull); // Optional, change based on business rules

            // Additional configurations or indices if needed
            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.SupplierName)  // Adding index on SupplierName for faster lookup
                .IsUnique();
        }

        public override int SaveChanges()
        {
            AddAuditInfo();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            AddAuditInfo();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddAuditInfo();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            AddAuditInfo();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void AddAuditInfo()
        {

            var modifiedEntries = ChangeTracker.Entries()
                .Where(x => x.Entity is IAuditableEntity &&
                           (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entry in modifiedEntries)
            {
                var entity = (IAuditableEntity)entry.Entity;
                var now = DateTime.Now;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = now;
                }
                else
                {
                    base.Entry(entity).Property(x => x.CreatedAt).IsModified = false;
                }

                entity.UpdatedAt = now;
            }
        }
    }
}
