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
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("Books");

            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(b => b.Description)
                .HasMaxLength(2000);

            builder.Property(b => b.ISBN)
                .HasMaxLength(20);

            builder.HasIndex(b => b.ISBN)
                .IsUnique();

            builder.Property(b => b.Price)
                .HasColumnType("decimal(18,2)");

            builder.Property(b => b.DiscountPercentage)
                .HasColumnType("decimal(5,2)");

            builder.Property(b => b.Language)
                .HasMaxLength(50);

            builder.Property(b => b.CoverImageUrl)
                .HasMaxLength(500);

            builder.Property(b => b.Format)
                .HasMaxLength(50);

            // Computed property --> ignore in database
            builder.Ignore(b => b.DiscountedPrice);

            // Many-to-One: Book --> Author
            builder.HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Many-to-One: Book --> Publisher
            builder.HasOne(b => b.Publisher)
                .WithMany(p => p.Books)
                .HasForeignKey(b => b.PublisherId)
                .OnDelete(DeleteBehavior.Restrict);

            // Many-to-Many: Book --> Categories
            // shadow entity 
            builder.HasMany(b => b.Categories)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "BookCategories",
                    bc => bc.HasOne<Category>().WithMany().HasForeignKey("CategoryId"),
                    bc => bc.HasOne<Book>().WithMany().HasForeignKey("BookId")
                );

            // One-to-Many: Book --> Reviews
            builder.HasMany(b => b.Reviews)
                .WithOne(r => r.Book)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Book --> OrderItems
            builder.HasMany(b => b.OrderItems)
                .WithOne(oi => oi.Book)
                .HasForeignKey(oi => oi.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-Many: Book --> CartItems
            builder.HasMany(b => b.CartItems)
                .WithOne(ci => ci.Book)
                .HasForeignKey(ci => ci.BookId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
