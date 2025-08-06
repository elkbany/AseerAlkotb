using AseerAlkotb.Domain.Entites.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;


namespace AseerAlkotb.Infrastructure.Configurations
{
    public class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.ToTable("Authors");

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.Bio)
                .HasMaxLength(2000);

            builder.Property(a => a.ImageUrl)
                .HasMaxLength(500);

            // One-to-Many: Author --> Books
            builder.HasMany(a => a.Books)
                .WithOne(b => b.Author)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(a => a.CountryCode)
                .HasConversion<string>();
        }
    }  
}
