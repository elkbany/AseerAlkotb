using AseerAlkotb.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Infrastructure.Configurations
{
    public class PaymentConfigurtion : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            //builder.Property(e => e.Status)
            //          .HasConversion<string>()
            //          .IsRequired();

            //    builder.Property(e => e.Method)
            //          .HasConversion<string>()
            //          .IsRequired();

                        // Configure decimal precision for Amount property
            builder.Property(p => p.Amount)
                .HasPrecision(18, 2);

            // Configure ProviderPayload as required
            builder.Property(p => p.ProviderPayload)
                .IsRequired()
                .HasDefaultValue(string.Empty);

        }
    }
}
