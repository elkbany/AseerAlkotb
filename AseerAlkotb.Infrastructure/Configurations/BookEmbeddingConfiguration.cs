using AseerAlkotb.Domain.Entites.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AseerAlkotb.Infrastructure.Configurations
{
    public class BookEmbeddingConfiguration : IEntityTypeConfiguration<BookEmbedding>
    {
        public void Configure(EntityTypeBuilder<BookEmbedding> builder)
        {
            builder.HasKey(be => be.Id);

            builder.Property(be => be.Content)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(be => be.ContentType)
                .IsRequired()
                .HasMaxLength(50);

            // نخزّن الـ float[] كـ JSON في nvarchar(max)
            builder.Property(be => be.Embedding)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Property(be => be.LastUpdated)
                .IsRequired();

            builder.HasOne(be => be.Book)
                .WithMany()
                .HasForeignKey(be => be.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(be => be.BookId);
            builder.HasIndex(be => be.ContentType);
            builder.HasIndex(be => be.LastUpdated);
        }
    }
}
