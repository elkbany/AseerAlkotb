using AseerAlkotb.Domain.Entites.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Infrastructure.Configurations
{
    public class QuoteConfiguration : IEntityTypeConfiguration<Quote>
    {
        public void Configure(EntityTypeBuilder<Quote> builder)
        {
            builder.ToTable("Quotes");

            builder.Property(q => q.Comment)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasOne(q => q.Book)
                .WithMany(b => b.Quotes)
                .HasForeignKey(q => q.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(q => q.Author)
                .WithMany(a => a.Quotes)
                .HasForeignKey(q => q.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(q => q.User)
                .WithMany(u => u.Quotes)
                .HasForeignKey(q => q.UserId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
