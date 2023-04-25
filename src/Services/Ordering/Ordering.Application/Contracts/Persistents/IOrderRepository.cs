using Ordering.Domain.Entities;

namespace Ordering.Application.Contracts.Persistents
{
    public interface IOrderRepository : IAsyncRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUserName(string userName);
    }
}
