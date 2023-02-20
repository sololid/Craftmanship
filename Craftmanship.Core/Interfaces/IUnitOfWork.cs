using Craftmanship.Core.Models;

namespace Craftmanship.Core.Interfaces
{
    public interface IUnitOfWork
    {
        IApplicationUserService ApplicationUser { get; }
        ICategoryService Category { get; }
        IOrderDetailsService OrderDetails { get; }
        IOrderService Orders { get; }
        IProductService Product { get; }
        IShoppingCartService ShoppingCart { get; }
        Task Save();
    }
}
