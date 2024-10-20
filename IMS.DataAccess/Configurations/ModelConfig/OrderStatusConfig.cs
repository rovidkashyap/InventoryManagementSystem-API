using IMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMS.DataAccess.Configurations.ModelConfig
{
    public class OrderStatusConfig : IEntityTypeConfiguration<OrderStatus>
    {
        public void Configure(EntityTypeBuilder<OrderStatus> builder)
        {
            // Table Name
            builder.ToTable("OrderStatus");

            // Primary key
            builder.HasKey(x => x.OrderStatusId);

            // Properties Configuration
            builder.Property(x => x.OrderStatusName)
                .IsRequired()   // Make Order Status Name Mandatory
                .HasMaxLength(100); // Maximum length of 100 character

            builder.Property(x => x.Description)
                .HasMaxLength(500); // Maximum length of 500 characters

            // Realtionships
            builder.HasMany(x => x.Orders)
                .WithOne(o => o.OrderStatus)
                .HasForeignKey(o => o.OrderStatusId);
        }
    }
}
