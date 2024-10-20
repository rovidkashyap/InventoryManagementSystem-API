using IMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMS.DataAccess.Configurations.ModelConfig
{
    public class InventoryConfig : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            // Table Name
            builder.ToTable("Inventory");

            // Primary Key
            builder.HasKey(x => x.InventoryId);
            builder.Property(x => x.InventoryId).HasDefaultValueSql("NEWID()");

            // Required Fields
            builder.Property(x => x.ProductId).IsRequired();
            builder.Property(x => x.SupplierId).IsRequired();
            builder.Property(x => x.QuantityAdded).IsRequired();
            builder.Property(x => x.QuantityRemoved).IsRequired();
            builder.Property(x => x.QuantityInStock).IsRequired();
            builder.Property(x => x.UnitPrice).IsRequired();
            builder.Property(c => c.TransactionTypeId).IsRequired();
            builder.Property(c => c.TransactionDate).IsRequired();

            // Optional Fields
            builder.Property(e => e.Remark).HasMaxLength(500);  // Set maximum length for optional remark

            // Relationships
            builder.HasOne(x => x.Product)
                .WithMany(p => p.Inventories)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Supplier)
                .WithMany(s => s.Inventories)
                .HasForeignKey(s => s.SupplierId)
                .OnDelete(DeleteBehavior.Restrict); // Stops the deleting entity which are dependent on other

            builder.HasOne(x => x.TransactionType)
                .WithMany(t => t.Inventories)
                .HasForeignKey(t => t.TransactionTypeId)
                .OnDelete(DeleteBehavior.Restrict); // Stops the deleting entity which are dependent on other

        }
    }
}
