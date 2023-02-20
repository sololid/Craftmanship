using Craftmanship.Core.Models;

namespace Craftmanship.Core.Interfaces
{
    public interface IShoppingCartService : IService<ShoppingCarts>
    {
        int IncrementCount(ShoppingCarts shoppingCart, int count);
        int DecrementCount(ShoppingCarts shoppingCart, int count);
    }
}