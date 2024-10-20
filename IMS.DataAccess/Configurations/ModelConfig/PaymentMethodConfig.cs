using IMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMS.DataAccess.Configurations.ModelConfig
{
    public class PaymentMethodConfig : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            // Table Name
            builder.ToTable("PaymentMethod");

            // Primary Key
            builder.HasKey(x => x.PaymentMethodId);
            builder.Property(x => x.PaymentMethodId).HasDefaultValueSql("NEWID()");

            // Required Fields
            builder.Property(x => x.PaymentMethodName).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.Property(x => x.IsActive).IsRequired();
            
            // Relationships
            builder.HasMany(x => x.Orders)
                .WithOne(o => o.PaymentMethod)
                .HasForeignKey(c => c.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict Delete to prevent orphand Orders

        }
    }
}
