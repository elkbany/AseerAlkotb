

using AseerAlkotb.Domain.Entites.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace AseerAlkotb.Infrastructure.Configurations
{
    public class LikeDisLikeConfiguration : IEntityTypeConfiguration<LikeDisLike>
    {
        public void Configure(EntityTypeBuilder<LikeDisLike> builder)
        {
            builder
                .HasOne(l => l.Review)
                .WithMany(r => r.LikeDisLikes)
                .HasForeignKey(l => l.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);
            builder
                .HasOne(ld => ld.User)
                .WithMany(u=>u.LikeDisLikes)
                .HasForeignKey(ld => ld.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //builder
            //    .HasOne(l => l.Quote)
            //    .WithMany(q => q.LikesDislikes)
            //    .HasForeignKey(l => l.QuoteId)
            //    .OnDelete(DeleteBehavior.Cascade);

            // Optional: prevent duplicate likes by same user
            builder
                .HasIndex(l => new { l.UserId, l.ReviewId })
                .IsUnique()
                .HasFilter("[ReviewId] IS NOT NULL");

            //builder.Entity<LikeDislike>()
            //    .HasIndex(l => new { l.UserId, l.QuoteId })
            //    .IsUnique()
            //    .HasFilter("[QuoteId] IS NOT NULL");
        }
    }
}
