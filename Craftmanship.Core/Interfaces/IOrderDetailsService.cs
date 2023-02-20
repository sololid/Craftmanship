using Craftmanship.Core.Models;

namespace Craftmanship.Core.Interfaces
{
    public interface IOrderDetailsService : IService<OrderDetails>
    {
        void Update(OrderDetails orderDetails);
    }
}