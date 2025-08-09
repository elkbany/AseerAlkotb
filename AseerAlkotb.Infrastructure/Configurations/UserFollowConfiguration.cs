using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Domain.Entites.Base;
using AseerAlkotb.Domain.Entites.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AseerAlkotb.Infrastructure.Configurations
{
    public class UserFollowConfiguration : IEntityTypeConfiguration<UserFollow>
    {
        public void Configure(EntityTypeBuilder<UserFollow> builder)
        {
            builder.ToTable("UserFollows");
            builder.HasKey(t => t.Id);

            builder.HasOne(uf => uf.User)
               .WithMany(u => u.Following)
               .HasForeignKey(uf => uf.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(uf => uf.Author)
             .WithMany(a => a.Followers)
             .HasForeignKey(uf => uf.AuthorId)
             .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(uf => uf.Publisher)
             .WithMany(p => p.Followers)
             .HasForeignKey(uf => uf.PublisherId)
             .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(uf => new { uf.UserId, uf.AuthorId })
            .IsUnique()
            .HasFilter("[AuthorId] IS NOT NULL");

            // For publisher follows
            builder.HasIndex(uf => new { uf.UserId, uf.PublisherId })
                .IsUnique()
                .HasFilter("[PublisherId] IS NOT NULL");

            builder.ToTable(tb => tb.HasCheckConstraint(               // is to prevwnt duplecatete because in sql Null!=Null
           "CK_UserFollow_SingleFollowType",
           "([AuthorId] IS NOT NULL AND [PublisherId] IS NULL) OR " +
           "([AuthorId] IS NULL AND [PublisherId] IS NOT NULL)"));
        }

        
    }
}
