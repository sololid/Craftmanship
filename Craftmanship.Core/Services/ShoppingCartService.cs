using Craftmanship.Core.Data;
using Craftmanship.Core.Interfaces;
using Craftmanship.Core.Models;

namespace Craftmanship.Core.Services
{
    public class ShoppingCartService : Service<ShoppingCarts>, IShoppingCartService
    {
        private AppDbContext _db;
        public ShoppingCartService(AppDbContext db) : base(db) 
        {
            _db = db;
        }

        public int DecrementCount(ShoppingCarts shoppingCart, int count)
        {
            shoppingCart.Count -= count;
            return shoppingCart.Count;
        }

        public int IncrementCount(ShoppingCarts shoppingCart, int count)
        {
            shoppingCart.Count += count;
            return shoppingCart.Count;
        }
    }
}
