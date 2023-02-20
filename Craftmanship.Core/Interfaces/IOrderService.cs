using Craftmanship.Core.Models;

namespace Craftmanship.Core.Interfaces
{
    public interface IOrderService : IService<Orders>
    {
        void Update(Orders orders);
        void UpdateOrderStatus (int id, string orderStatus, string? paymentStatus = null);
        void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId);

	}
}