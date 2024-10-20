using IMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMS.DataAccess.Configurations.ModelConfig
{
    public class SupplierConfig : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            // Table Name
            builder.ToTable("Supplier");

            // Primary Key
            builder.HasKey(x => x.SupplierId);
            builder.Property(x => x.SupplierId).HasDefaultValueSql("NEWID()");

            // Required Fields
            builder.Property(x => x.SupplierName)
                .IsRequired()   // Supplier Name is required
                .HasMaxLength(200); //Set Maximum Characters for this field

            builder.Property(x => x.ContactName)
                .IsRequired()  // Supplier Contact Person is required
                .HasMaxLength(200);  // Set Maximum Length for Contact person

            builder.Property(x => x.PhoneNumber)
                .HasMaxLength(12); // Set Maximum characters for Phone Number

            builder.Property(x => x.Email)
                .IsRequired();  // Set Email for Supplier

            builder.Property(x => x.Notes)
                .HasMaxLength(500);     // Set maximum characters for notes

            // Relationships
            builder.HasMany(x => x.Inventories)
                .WithOne(m => m.Supplier)
                .HasForeignKey(e => e.SupplierId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict delete to prevent orphaned products

        }
    }
}
