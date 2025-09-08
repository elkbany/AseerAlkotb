using AseerAlkotb.Domain.Entites.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AseerAlkotb.Infrastructure.Configurations
{
    public class RagQueryConfiguration : IEntityTypeConfiguration<RagQuery>
    {
        public void Configure(EntityTypeBuilder<RagQuery> builder)
        {
            builder.HasKey(rq => rq.Id);

            builder.Property(rq => rq.Query)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(rq => rq.Response)
                .IsRequired()
                .HasMaxLength(5000);

            builder.Property(rq => rq.UserId)
                .HasMaxLength(450);

            builder.Property(rq => rq.QueryType)
                .IsRequired();

            builder.Property(rq => rq.SimilarityScore)
                .IsRequired()
                .HasColumnType("float");

            builder.Property(rq => rq.CreatedAt)
                .IsRequired();

            builder.HasIndex(rq => rq.CreatedAt);
            builder.HasIndex(rq => rq.QueryType);
            builder.HasIndex(rq => rq.UserId);
        }
    }
}
