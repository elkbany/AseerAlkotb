﻿using AseerAlkotb.Domain.Entites;
using AseerAlkotb.Domain.Entites.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Infrastructure.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.ShippingCost)
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.TaxAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.DiscountAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.Governorate)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(o => o.TrackingNumber)
                .HasMaxLength(100);

            builder.HasIndex(o => o.TrackingNumber)
                .IsUnique();

            // One-to-Many: Order --> OrderItems
            builder.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);
          
                      builder.Property(o => o.DiscountAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.FinalAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.Governorate)
                .IsRequired()
                .HasMaxLength(500);

            // One-to-One: Order --> Payment
            // Payment takes PaymentMethod from Order (per project specification)
            // Use Restrict to prevent accidental Order deletion when Payment exists
            builder.HasOne(o => o.Payment)
                   .WithOne(p => p.Order)
                   .HasForeignKey<Payment>(p => p.OrderId)   // Payment has the FK
                   .OnDelete(DeleteBehavior.Restrict);

            // Note: Restrict prevents Order deletion if Payment exists
            // This ensures data integrity and follows business logic

            //builder.HasOne(o => o.Payment)
            //   .WithOne(p => p.Order)
            //   .HasForeignKey<Order>(o => o.PayId) // Foreign key in Order pointing to Payment
            //   .OnDelete(DeleteBehavior.SetNull); // if Payment is deleted, keep the Order but set PayId to null


        }
    }
}
