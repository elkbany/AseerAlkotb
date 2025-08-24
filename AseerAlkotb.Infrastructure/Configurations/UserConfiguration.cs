using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Domain.Entites.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace AseerAlkotb.Infrastructure.Configurations
{
    class UserConfiguration : IEntityTypeConfiguration<User>
    {
       
         public void Configure(EntityTypeBuilder<User> builder)
            {
                builder.ToTable("Users");

                builder.Property(a => a.Bio)
                    .HasMaxLength(2000);

                builder.Property(a => a.ProfilePictureUrl)
                .HasMaxLength(500);

                builder.HasOne(a => a.Cart)
                     .WithOne(b => b.User)
                     .HasForeignKey<Cart>(c => c.UserId)
                     .OnDelete(DeleteBehavior.Restrict);


         }
        
    }
}
