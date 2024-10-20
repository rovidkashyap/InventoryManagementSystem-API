using IMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMS.DataAccess.Configurations.ModelConfig
{
    public class OrderItemConfig : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            // Table Name
            builder.ToTable("OrderItems");

            // Primary Key
            builder.HasKey(x => x.OrderItemId);
            builder.Property(x => x.OrderItemId).HasDefaultValueSql("NEWID()");

            // Required Fields
            builder.Property(x => x.OrderId).IsRequired();
            builder.Property(x => x.ProductId).IsRequired();
            builder.Property(x => x.Quantity).IsRequired();

            builder.Property(x => x.UnitPrice).IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.TotalPrice).IsRequired()
                .HasColumnType("decimal(18,2)"); ; // Automatically Calculates TotalPrice based on Quantity and UnitPrice

            builder.Property(x => x.DiscountAmount).IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.TaxAmount).IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.NetPrice).IsRequired()
                .HasColumnType("decimal(18,2)");
            
            // Relationships
            builder.HasOne(x => x.Order)
                .WithMany(c => c.OrderItems)
                .HasForeignKey(c => c.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // Delete Related OrderItems when an Order is deleted

            builder.HasOne(x => x.Product)
                .WithMany(c => c.OrderItems)
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a Product if it has related OrderItems
        }
    }
}
