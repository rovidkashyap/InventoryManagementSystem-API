using IMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMS.DataAccess.Configurations.ModelConfig
{
    public class CustomerConfig : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            // Table Name
            builder.ToTable("Customer");

            // Primary Key
            builder.HasKey(x => x.CustomerId);
            builder.Property(x => x.CustomerId).HasDefaultValueSql("NEWID()");

            // Properties Configuration
            builder.Property(x => x.CustomerName)
                .IsRequired()   // Make Customer Name Mandatory
                .HasMaxLength(150); // Maximum length of 150 character

            builder.Property(x => x.Email)
                .IsRequired()   // Make Email Mandatory
                .HasMaxLength(100);  // Maximum lengt hof 100 characters

            builder.Property(x => x.PhoneNumber)
                .HasMaxLength(12);  // Maximum Length of 12 characters

            builder.Property(x => x.WhatsAppNumber)
                .HasMaxLength(12);  // Maximum Length of 12 characters

            builder.Property(c => c.Gender)
                .IsRequired() // Make Gender mandatory
                .HasMaxLength(10); // Maximum length of 10 characters

            builder.Property(c => c.Address)
                .IsRequired() // Make Address mandatory
                .HasMaxLength(200); // Maximum length of 200 characters

            builder.Property(c => c.City)
                .IsRequired() // Make City mandatory
                .HasMaxLength(100); // Maximum length of 100 characters

            builder.Property(c => c.State)
                .IsRequired() // Make State mandatory
                .HasMaxLength(100); // Maximum length of 100 characters

            builder.Property(c => c.PostalCode)
                .IsRequired() // Make PostalCode mandatory
                .HasMaxLength(20); // Maximum length of 20 characters

            builder.Property(c => c.Country)
                .IsRequired() // Make Country mandatory
                .HasMaxLength(100); // Maximum length of 100 characters

            builder.Property(c => c.DateOfBirth)
                .IsRequired(); // Make DateOfBirth mandatory

            builder.Property(c => c.Notes)
                .HasMaxLength(500); // Maximum length of 500 characters (nullable)

            // Relationships
            builder.HasMany(x => x.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.OrderId)
                .OnDelete(DeleteBehavior.Cascade);  // Define delete behavior
        }
    }
}
