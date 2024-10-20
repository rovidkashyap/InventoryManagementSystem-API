using IMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMS.DataAccess.Configurations.ModelConfig
{
    public class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            // Table Name
            builder.ToTable("Category");

            // Primary Key
            builder.HasKey(x => x.CategoryId);
            builder.Property(x => x.CategoryId).HasDefaultValueSql("NEWID()");

            // Properties Configuration
            builder.Property(x => x.CategoryName)
                .IsRequired()   // Make Category Name Mandatory
                .HasMaxLength(150); // Maximum length of 150 character

            builder.Property(c => c.Description)
                .HasMaxLength(500); // Maximum length of 500 characters (nullable)

            // Relationships
            builder.HasMany(x => x.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);  // Define delete behavior

        }
    }
}
