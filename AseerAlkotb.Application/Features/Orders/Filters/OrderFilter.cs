using AseerAlkotb.Application.Features.Orders.Requests;
using AseerAlkotb.Domain.Entites.Models;


namespace AseerAlkotb.Application.Features.Orders.Filters
{
    public static class OrderFilter
    {
        public static IQueryable<Order> Filter(this IQueryable<Order> query, GetAllOrdersPaginatedRequest request)
        {
            if (request.OrderStatus != null)
            {
                query = query.Where(o => o.Status == request.OrderStatus);
            }
            if (request.GovernorateId != null)
            {
                query=query.Where(o=>o.GovernorateId == request.GovernorateId);
            }
            if (!request.DateAscending)
            {
               query=query.OrderByDescending(o=>o.OrderDate);
            }
            return query;
        }


    }
}
