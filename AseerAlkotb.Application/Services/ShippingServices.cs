﻿using AseerAlkotb.Application.Features.Orders.Requests;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Enums;
using AseerAlkotb.Domain.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Application.Services
{
    public static class ShippingServices
    {
        public const decimal FREE_SHIPPING_THRESHOLD = 600m; // EGP
        public const decimal EXPRESS_DELIVERY_MULTIPLIER = 1.5m;

        // Default shipping rates by governorate name (fallback when no specific rate is configured)
        private static readonly Dictionary<string, decimal> DEFAULT_SHIPPING_RATES = new()
        {
            // Cairo and nearby areas
            { "القاهرة", 25m },
            { "الجيزة", 30m },
            { "القليوبية", 35m },
            
            // Alexandria and coastal areas
            { "الإسكندرية", 45m },
            { "السويس", 50m },
            { "بورسعيد", 55m },
            { "الإسماعيلية", 50m },
            { "الأقصر", 65m },
            
            // Delta regions
            { "المنوفية", 40m },
            { "الغربية", 45m },
            { "الدقهلية", 50m },
            { "البحيرة", 50m },
            { "كفر الشيخ", 55m },
            { "دمياط", 60m },
            { "الشرقية", 50m },
            
            // Upper Egypt
            { "بني سويف", 45m },
            { "الفيوم", 50m },
            { "المنيا", 60m },
            { "أسيوط", 70m },
            { "سوهاج", 75m },
            { "قنا", 80m },
            { "أسوان", 85m },
            
            // Remote areas
            { "البحر الأحمر", 90m },
            { "الوادي الجديد", 100m },
            { "مطروح", 85m },
            { "شمال سيناء", 110m },
            { "جنوب سيناء", 95m }
        };

        public static async Task<decimal> GetShippingRateAsync(int governorateId, IUnitOfWork unitOfWork)
        {
            var governorate = await unitOfWork.Governorates.GetByIdAsync(governorateId);
            if (governorate == null)
                return 50m; // Default rate

            return DEFAULT_SHIPPING_RATES.TryGetValue(governorate.Name, out var rate) ? rate : 50m;
        }

        public static async Task<decimal> CalculateShippingCostAsync(AddOrderRequest request, decimal totalAmount, IUnitOfWork unitOfWork)
        {
            var standardRate = await GetShippingRateAsync(request.GovernorateId, unitOfWork);
            var isFreeShipping = totalAmount >= FREE_SHIPPING_THRESHOLD;
            if (isFreeShipping)
                standardRate = 0m;
            return standardRate;
        }

        public static async Task<decimal> GetShippingCostForAGovernorateAsync(int governorateId, IUnitOfWork unitOfWork)
        {
            return await GetShippingRateAsync(governorateId, unitOfWork);
        }
    }
}
