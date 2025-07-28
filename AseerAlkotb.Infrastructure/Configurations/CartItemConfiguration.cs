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
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable("CartItems");

            builder.Property(ci => ci.UnitPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(ci => ci.Quantity)
                .HasDefaultValue(1);

            // Computed property --> ignore in database
            builder.Ignore(ci => ci.TotalPrice);

            // Composite key: UserId + BookId (one user can't have same book twice in cart)
            builder.HasKey(ci => new { ci.UserId, ci.BookId });

            // Many-to-One: CartItem --> Book
            builder.HasOne(ci => ci.Book)
                .WithMany(b => b.CartItems)
                .HasForeignKey(ci => ci.BookId)
                .OnDelete(DeleteBehavior.Cascade);

          
            //builder.HasOne(ci => ci.User)
            //    .WithMany(u => u.CartItems)
            //    .HasForeignKey(ci => ci.UserId)
            //    .OnDelete(DeleteBehavior.Cascade);
            
        }
    }
}
