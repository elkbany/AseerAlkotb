using System.Text.Json;
using AseerAlkotb.Domain.Entites.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AseerAlkotb.Infrastructure.Configurations
{
    public class BookEmbeddingConfiguration : IEntityTypeConfiguration<BookEmbedding>
    {
        public void Configure(EntityTypeBuilder<BookEmbedding> builder)
        {
            builder.HasKey(be => be.Id);

            builder.Property(be => be.Content).IsRequired().HasMaxLength(4000);
            builder.Property(be => be.ContentType).IsRequired().HasMaxLength(50);

            var conv = new ValueConverter<float[], string>(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<float[]>(v, (JsonSerializerOptions?)null) ?? Array.Empty<float>()
            );

            builder.Property(be => be.Embedding)
                   .HasConversion(conv)
                   .HasColumnType("nvarchar(max)")
                   .IsRequired();

            builder.Property(be => be.ChunkIndex);
            builder.Property(be => be.TokenCount);
            builder.Property(be => be.LastUpdated).IsRequired();

            builder.HasOne(be => be.Book)
                   .WithMany()
                   .HasForeignKey(be => be.BookId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(be => new { be.BookId, be.ContentType, be.ChunkIndex });
            builder.HasIndex(be => be.LastUpdated);
        }
    }
}
