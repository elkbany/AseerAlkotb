using AseerAlkotb.Domain.Entites.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Infrastructure.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {

            builder.ToTable("OrderItems");
            builder.HasKey(oi => new { oi.OrderId, oi.BookId });

            builder.Property(oi => oi.UnitPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(oi => oi.Quantity)
                .HasDefaultValue(1);

            // Computed property --> ignore in database
            builder.Ignore(oi => oi.TotalPrice);

            // Many-to-One: OrderItem --> Order
            builder.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-One: OrderItem --> Book
            builder.HasOne(oi => oi.Book)
                .WithMany(b => b.OrderItems)
                .HasForeignKey(oi => oi.BookId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
