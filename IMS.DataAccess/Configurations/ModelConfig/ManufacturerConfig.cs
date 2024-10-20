using IMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMS.DataAccess.Configurations.ModelConfig
{
    public class ManufacturerConfig : IEntityTypeConfiguration<Manufacturer>
    {
        public void Configure(EntityTypeBuilder<Manufacturer> builder)
        {
            // Table Name
            builder.ToTable("Manufacturer");

            // Primary Key
            builder.HasKey(x => x.ManufacturerId);
            builder.Property(x => x.ManufacturerId).HasDefaultValueSql("NEWID()");

            // Required Fields
            builder.Property(x => x.ManufacturerName).IsRequired();

            // Optional Fields
            builder.Property(e => e.Address).HasMaxLength(500);  // Set maximum length for optional field

            // Relationship
            builder.HasMany(x => x.Products)
                .WithOne(p => p.Manufacturer)
                .HasForeignKey(p => p.ManufacturerId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict deletion of Manufacturer if product exist
                
        }
    }
}
