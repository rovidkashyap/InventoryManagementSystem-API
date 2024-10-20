using IMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMS.DataAccess.Configurations.ModelConfig
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            // Table Name
            builder.ToTable("Product");

            // Primary Key
            builder.HasKey(x => x.ProductId);
            builder.Property(x => x.ProductId).HasDefaultValueSql("NEWID()");

            // Required Fields
            builder.Property(x => x.ProductName)
                .IsRequired()   // Product Name is required
                .HasMaxLength(200); //Set Maximum Characters for this field

            builder.Property(x => x.ProductCode)
                .IsRequired()  // Product Code is required
                .HasMaxLength(200);  // Set Maximum Length for Product Code

            builder.Property(x => x.Description)
                .HasMaxLength(500); // Set Maximum characters for description

            builder.Property(x => x.Price)
                .IsRequired() // Price is required
                .HasColumnType("decimal(18,2)");    // Setting precison for price

            builder.Property(e => e.Weight)
                .HasColumnType("decimal(18,2)"); // Setting precision for Weight (optional)

            builder.Property(e => e.Dimensions)
                .HasMaxLength(100); // Maximum length for Dimensions (optional)

            builder.Property(e => e.ReorderLevel)
                .IsRequired(); // ReorderLevel is required

            builder.Property(e => e.IsActive)
                .IsRequired(); // IsActive is required

            builder.Property(e => e.ImagePath)
                .HasMaxLength(500); // Maximum length for ImagePath (optional)

            builder.HasIndex(x => x.ProductCode)
                .IsUnique();    // Ensure ProductCode is unique

            // Relationships
            builder.HasOne(x => x.Manufacturer)
                .WithMany(m => m.Products)
                .HasForeignKey(e => e.ManufacturerId);
            //.OnDelete(DeleteBehavior.Restrict); // Restrict delete to prevent orphaned products

            builder.HasOne(x => x.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(e => e.CategoryId);
            //.OnDelete(DeleteBehavior.Restrict); // Restrict delete to prevent orphaned products

            builder.HasOne(x => x.UnitOfMeasure)
                .WithMany(c => c.Products)
                .HasForeignKey(e => e.UnitOfMeasureId);
                //.OnDelete(DeleteBehavior.Restrict); // Restrict delete to prevent orphaned products

            builder.HasMany(x => x.Inventories)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete to remove related inventories

            builder.HasMany(x => x.OrderItems)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict delete to prevent orphaned order items

        }
    }
}
