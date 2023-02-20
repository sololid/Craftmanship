using Craftmanship.Core.Data;
using Craftmanship.Core.Interfaces;
using Craftmanship.Core.Models;
using System.Runtime.CompilerServices;

namespace Craftmanship.Core.Services
{
    public class OrderDetailService : Service<OrderDetails>, IOrderDetailsService
    {
        private AppDbContext _db;

        public OrderDetailService(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderDetails orderDetails)
        {
            _db.OrderDetails.Update(orderDetails);
        }
    }
}
