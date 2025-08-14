using AseerAlkotb.Application.Features.Orders.Requests;
using AseerAlkotb.Domain.Enums;
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

     
        private static readonly Dictionary<EgyptGovernorates, decimal> SHIPPING_RATES_FROM_CAIRO = new()
        {
       
            { EgyptGovernorates.AL_QAHIRAH, 25m }, 

          
            { EgyptGovernorates.AL_JIZAH, 30m },         
            { EgyptGovernorates.AL_QALYUBIYAH, 35m },     

      
            { EgyptGovernorates.AL_ISKANDARIYAH, 45m },   
            { EgyptGovernorates.AS_SUWAYS, 50m }, 
            { EgyptGovernorates.BUR_SAID, 55m },       
            { EgyptGovernorates.AL_ISMAILIA, 50m },   
            { EgyptGovernorates.AL_UQSUR, 65m },           

         
            { EgyptGovernorates.AL_MINUFIYAH, 40m },     
            { EgyptGovernorates.AL_GHARBIYAH, 45m },   
            { EgyptGovernorates.AD_DAQAHLIYAH, 50m }, 
            { EgyptGovernorates.AL_BUHAYRAH, 50m },  
            { EgyptGovernorates.KAFR_ASH_SHAYKH, 55m },     
            { EgyptGovernorates.DUMYAT, 60m },   
            { EgyptGovernorates.ASH_SHARQIYAH, 50m },

       
            { EgyptGovernorates.BANI_SUWAYF, 45m },       
            { EgyptGovernorates.AL_FAYYUM, 50m },          
            { EgyptGovernorates.AL_MINYA, 60m },    
            { EgyptGovernorates.ASYUT, 70m },              
            { EgyptGovernorates.SUHAJ, 75m },      
            { EgyptGovernorates.QINA, 80m },          
            { EgyptGovernorates.ASWAN, 85m },             

            { EgyptGovernorates.AL_BAHR_AL_AHMAR, 90m },  
            { EgyptGovernorates.AL_WADI_AL_JADID, 100m }, 
            { EgyptGovernorates.MATRUH, 85m },             
            { EgyptGovernorates.SHAMAL_SINA, 110m },      
            { EgyptGovernorates.JANUB_SINA, 95m }         
        };


        public static decimal GetShippingRate(EgyptGovernorates governorate)
        {
            return SHIPPING_RATES_FROM_CAIRO.TryGetValue(governorate, out var rate) ? rate : 50m; 
        }
        public static decimal CalculateShippingCost(AddOrderRequest request)
        {
            var standardRate = GetShippingRate(request.Governorate);
            //var isFreeShipping = request.OrderTotal >= FREE_SHIPPING_THRESHOLD;
             var result = standardRate;
            return result;
        }
    }
}
