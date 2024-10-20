using IMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMS.DataAccess.Configurations.ModelConfig
{
    public class UnitOfMeasureConfig : IEntityTypeConfiguration<UnitOfMeasure>
    {
        public void Configure(EntityTypeBuilder<UnitOfMeasure> builder)
        {
            // Table Name
            builder.ToTable("UnitOfMeasure");

            // Primary Key
            builder.HasKey(x => x.UnitOfMeasureId);
            builder.Property(x => x.UnitOfMeasureId).HasDefaultValueSql("NEWID()");

            // Required Fields
            builder.Property(x => x.UnitName)
                .IsRequired();  // Name is required

            // Relationships
            builder.HasMany(x => x.Products)
                .WithOne(c => c.UnitOfMeasure)
                .HasForeignKey(c => c.UnitOfMeasureId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict delete to prevent orphaned products
        }
    }
}
