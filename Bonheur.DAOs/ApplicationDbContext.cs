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
        public DbSet<AdPackage> AdPackages { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageAttachment> MessageAttachments { get; set; }  
        public DbSet<FavoriteSupplier> FavoriteSuppliers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<SocialNetwork> SocialNetworks { get; set; }
        public DbSet<SupplierSocialNetwork> SupplierSocialNetworks { get; set; }
        public DbSet<SupplierFAQ> SupplierFAQs { get; set; }



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

            // Configure Supplier -> User (many-to-one relationship)
            modelBuilder.Entity<Supplier>()
                .HasOne(s => s.User)
                .WithMany()  // Assuming one-to-many relationship with ApplicationUser
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Supplier -> SupplierCategory (many-to-one relationship)
            modelBuilder.Entity<Supplier>()
                .HasOne(s => s.Category)
                .WithMany()
                .HasForeignKey(s => s.CategoryId)
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
                .WithMany(s => s.Images)
                .HasForeignKey(si => si.SupplierId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure RequestPricing -> Supplier (many-to-one relationship)
            modelBuilder.Entity<RequestPricing>()
                .HasOne(rp => rp.Supplier)
                .WithMany()
                .HasForeignKey(rp => rp.SupplierId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RequestPricing>()
               .HasOne(rp => rp.User)
               .WithMany()
               .HasForeignKey(rp => rp.UserId)
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
                .OnDelete(DeleteBehavior.SetNull);

            // Additional configurations or indices if needed
            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.Name)  // Adding index on SupplierName for faster lookup
                .IsUnique();

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.Status);  // Adding index on SupplierStatus for faster lookup

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.Priority);  // Adding index on SupplierPriority for faster lookup

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.IsFeatured);  // Adding index on SupplierIsFeatured for faster lookup

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.OnBoardStatus);  // Adding index on SupplierOnBoardStatus for faster lookup

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.SubscriptionPackageId);  // Adding index on SupplierSubscriptionPackageId for faster lookup

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.UserId);  // Adding index on SupplierUserId for faster lookup

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.CategoryId);  // Adding index on SupplierCategoryId for faster lookup

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.CreatedAt);  // Adding index on SupplierCreatedAt for faster lookup

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.UpdatedAt);  // Adding index on SupplierUpdatedAt for faster lookup

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.PriorityEnd);  // Adding index on SupplierProrityEnd for faster lookup

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.Discount);  // Adding index on SupplierDiscount for faster lookup

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.AverageRating);  // Adding index on SupplierAverageRating for faster lookup

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.Ward);  // Adding index on SupplierWard for faster lookup

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.District);  // Adding index on SupplierDistrict for faster lookup

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.Province);  // Adding index on SupplierProvince for faster lookup

            // Configure Message -> Sender (many-to-one relationship)
            modelBuilder.Entity<Message>()
                .HasOne(qm => qm.Sender)
                .WithMany()
                .HasForeignKey(qm => qm.SenderId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascading delete for Sender

            modelBuilder.Entity<Message>()
                .HasOne(qm => qm.Receiver)
                .WithMany()
                .HasForeignKey(qm => qm.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascading delete for Receiver

            // Configure MessageAttachment -> Message (many-to-one relationship)
            modelBuilder.Entity<MessageAttachment>()
                .HasOne(qma => qma.Message)
                .WithMany(qm => qm.Attachments)
                .HasForeignKey(qma => qma.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Advertisement -> AdPackage (many-to-one relationship)
            modelBuilder.Entity<Advertisement>()
                .HasOne(a => a.AdPackage)
                .WithMany()
                .HasForeignKey(a => a.AdPackageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Additional index or configuration for Advertisement entity
            modelBuilder.Entity<Advertisement>()
                .HasIndex(a => a.IsActive);  // Adding index on AdvertisementIsActive for faster lookup

            modelBuilder.Entity<AdPackage>()
                .Property(u => u.AdType)
                .HasConversion(new EnumToStringConverter<AdType>());

            // Configure FavoriteSupplier Table
            modelBuilder.Entity<FavoriteSupplier>(entity =>
            {
                entity.HasKey(fs => fs.Id); 

                entity.HasOne(fs => fs.User)
                      .WithMany(u => u.FavoriteSuppliers)
                      .HasForeignKey(fs => fs.UserId)
                      .OnDelete(DeleteBehavior.Cascade); 

                entity.HasOne(fs => fs.Supplier)
                      .WithMany()
                      .HasForeignKey(fs => fs.SupplierId)
                      .OnDelete(DeleteBehavior.Cascade); 

                entity.ToTable("FavoriteSuppliers");
            });

            // Configure Order -> User (many-to-one relationship)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
               .HasOne(o => o.Supplier)
               .WithMany()
               .HasForeignKey(o => o.SupplierId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion(new EnumToStringConverter<OrderStatus>());

            modelBuilder.Entity<Order>()
               .Property(o => o.PaymentMethod)
               .HasConversion(new EnumToStringConverter<PaymentMethod>());

            modelBuilder.Entity<Order>()
               .Property(o => o.PaymentStatus)
               .HasConversion(new EnumToStringConverter<PaymentStatus>());

            // Configure OrderDetail -> Order (many-to-one relationship)
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure OrderDetail -> SubscriptionPackage (optional one-to-one or many-to-one relationship)
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.SubscriptionPackage)
                .WithMany()
                .HasForeignKey(od => od.SubscriptionPackageId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure OrderDetail -> AdPackage (optional one-to-one or many-to-one relationship)
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.AdPackage)
                .WithMany()
                .HasForeignKey(od => od.AdPackageId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Invoice -> Order (many-to-one relationship)
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Order)
                .WithOne(o => o.Invoice)
                .HasForeignKey<Invoice>(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Invoice>()
               .HasOne(od => od.User)
               .WithMany()
               .HasForeignKey(od => od.UserId)
               .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Invoice>()
               .HasOne(od => od.Supplier)
               .WithMany()
               .HasForeignKey(od => od.SupplierId)
               .OnDelete(DeleteBehavior.SetNull);

            // Thiết lập quan hệ giữa Supplier và SupplierFAQ (1 - N)
            modelBuilder.Entity<SupplierFAQ>()
                .HasOne(f => f.Supplier)
                .WithMany(s => s.Faqs)
                .HasForeignKey(f => f.SupplierId)
                .OnDelete(DeleteBehavior.Cascade);

            // Thiết lập quan hệ giữa Supplier và SupplierSocialNetwork (1 - N)
            modelBuilder.Entity<SupplierSocialNetwork>()
                .HasOne(ssn => ssn.Supplier)
                .WithMany(s => s.SocialNetworks)
                .HasForeignKey(ssn => ssn.SupplierId)
                .OnDelete(DeleteBehavior.Cascade);

            // Thiết lập quan hệ giữa SocialNetwork và SupplierSocialNetwork (1 - N)
            modelBuilder.Entity<SupplierSocialNetwork>()
                .HasOne(ssn => ssn.SocialNetwork)
                .WithMany()
                .HasForeignKey(ssn => ssn.SocialNetworkId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
            .HasOne(r => r.Supplier)
            .WithMany(s => s.Reviews) // Supplier có nhiều Review
            .HasForeignKey(r => r.SupplierId)
            .OnDelete(DeleteBehavior.Cascade);


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
                var now = DateTimeOffset.UtcNow;

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
