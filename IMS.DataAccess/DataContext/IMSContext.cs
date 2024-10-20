using IMS.Core.BaseEntity;
using IMS.Core.Entities;
using IMS.DataAccess.Configurations.ModelConfig;
using IMS.DataAccess.Seed;
using Microsoft.EntityFrameworkCore;

namespace IMS.DataAccess.DataContext
{
    public class IMSContext : DbContext
    {
        public IMSContext(DbContextOptions<IMSContext> options)
            :base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerConfig());
            modelBuilder.ApplyConfiguration(new CategoryConfig());
            modelBuilder.ApplyConfiguration(new InventoryConfig());
            modelBuilder.ApplyConfiguration(new ManufacturerConfig());
            modelBuilder.ApplyConfiguration(new OrderConfig());
            modelBuilder.ApplyConfiguration(new OrderItemConfig());
            modelBuilder.ApplyConfiguration(new PaymentMethodConfig());
            modelBuilder.ApplyConfiguration(new ProductConfig());
            modelBuilder.ApplyConfiguration(new SupplierConfig());
            modelBuilder.ApplyConfiguration(new TransactionTypeConfig());
            modelBuilder.ApplyConfiguration(new UnitOfMeasureConfig());
            modelBuilder.ApplyConfiguration(new OrderStatusConfig());

            // Seed data
            SeedData.Seed(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void UpdateAuditFields()
        {
            var entities = ChangeTracker.Entries<AuditableEntity>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    entity.Entity.CreatedDate = DateTime.UtcNow;
                    entity.Entity.CreatedBy = "Rovid Kashyap";
                }

                if (entity.State == EntityState.Modified)
                {
                    entity.Entity.UpdatedDate = DateTime.UtcNow;
                    entity.Entity.UpdatedBy = "Rovid";
                }
            }
        }
    }
}
