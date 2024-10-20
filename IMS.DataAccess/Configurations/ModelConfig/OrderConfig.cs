using IMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMS.DataAccess.Configurations.ModelConfig
{
    public class OrderConfig : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            // Table Name
            builder.ToTable("Orders");

            // Primary Key
            builder.HasKey(x => x.OrderId);
            builder.Property(x => x.OrderId).HasDefaultValueSql("NEWID()");

            // Required Fields
            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.OrderDate).IsRequired();
            builder.Property(x => x.TotalPrice).IsRequired();
            builder.Property(x => x.PaymentMethodId).IsRequired();
            builder.Property(x => x.OrderStatusId).IsRequired();
            builder.Property(x => x.ShippingAddress).IsRequired();
            builder.Property(x => x.BillingAddress).IsRequired(false);
            builder.Property(x => x.BillingEmail).IsRequired();
            builder.Property(x => x.BillingPhone).IsRequired();

            builder.Property(x => x.TotalPrice).IsRequired()
                .HasColumnType("decimal(18, 2)");

            builder.Property(x => x.TaxAmount).IsRequired()
                .HasColumnType("Decimal(18, 2)")
                .HasDefaultValue(0);

            builder.Property(x => x.DiscountAmount).IsRequired()
                .HasColumnType("decimal(18, 2)")
                .HasDefaultValue(0);

            builder.Property(x => x.ShippingCost).IsRequired()
                .HasColumnType("decimal(18, 2)")
                .HasDefaultValue(0);

            builder.Property(x => x.NetAmount).IsRequired()
                .HasColumnType("decimal(18, 2)");

            builder.Property(x => x.DeliveredDate)
                .IsRequired(false);

            // Optional Fields
            builder.Property(e => e.Notes).HasMaxLength(500);  // Set maximum length for optional remark

            // Relationships
            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.PaymentMethod)
                .WithMany()
                .HasForeignKey(c => c.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.OrderStatus)
                .WithMany()
                .HasForeignKey(o => o.OrderStatusId);

            builder.HasMany(x => x.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId);
        }
    }
}
