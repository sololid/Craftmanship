using Craftmanship.Core.Data;
using Craftmanship.Core.Interfaces;
using Craftmanship.Core.Models;

namespace Craftmanship.Core.Services
{
    public class OrderService : Service<Orders>, IOrderService
    {
        private AppDbContext _db;
        public OrderService(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Orders orders)
        {
            _db.Orders.Update(orders);
        }

        public void UpdateOrderStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = _db.Orders.FirstOrDefault(x => x.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (paymentStatus != null) 
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }
        
        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
        {
            var orderFromDb = _db.Orders.FirstOrDefault(x => x.Id == id);
            orderFromDb.PaymentDate = DateTime.Now;
            orderFromDb.SessionId = sessionId;
            orderFromDb.PaymentIntentId = paymentIntentId;
        }
    }
}
