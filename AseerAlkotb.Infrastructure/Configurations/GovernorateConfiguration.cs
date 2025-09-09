using AseerAlkotb.Domain.Entites.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AseerAlkotb.Infrastructure.Configurations
{
    public class GovernorateConfiguration : IEntityTypeConfiguration<Governorate>
    {
        public void Configure(EntityTypeBuilder<Governorate> builder)
        {
            builder.ToTable("Governorates");

            builder.Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(200);

            // One-to-Many: Governorate --> Cities
            builder.HasMany(g => g.Cities)
                .WithOne(c => c.Governorate)
                .HasForeignKey(c => c.GovernorateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}