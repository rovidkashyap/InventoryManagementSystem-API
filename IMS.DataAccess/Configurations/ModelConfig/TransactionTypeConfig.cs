using IMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMS.DataAccess.Configurations.ModelConfig
{
    public class TransactionTypeConfig : IEntityTypeConfiguration<TransactionType>
    {
        public void Configure(EntityTypeBuilder<TransactionType> builder)
        {
            // Table Name
            builder.ToTable("TransactionType");

            // Primary Key
            builder.HasKey(x => x.TransactionTypeId);
            builder.Property(x => x.TransactionTypeId).HasDefaultValueSql("NEWID()");

            // Required Fields
            builder.Property(x => x.Name)
                .IsRequired();  // Name is required

            builder.Property(x => x.Description)
                .HasMaxLength(500); // Set maximum characters for description

            // Relationships
            builder.HasMany(x => x.Inventories)
                .WithOne(c => c.TransactionType)
                .HasForeignKey(c => c.TransactionTypeId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict delete to prevent orphaned inventories
        }
    }
}
