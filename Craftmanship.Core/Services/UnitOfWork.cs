using Craftmanship.Core.Data;
using Craftmanship.Core.Interfaces;
using Craftmanship.Core.Models;

namespace Craftmanship.Core.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;

        public UnitOfWork(AppDbContext db)
        {
            _db = db;
            Category = new CategoryService(_db);
            ApplicationUser = new ApplicationUserService(_db);
            OrderDetails = new OrderDetailService(_db);
            Orders = new OrderService(_db);
            Product = new ProductService(_db);
            ShoppingCart = new ShoppingCartService(_db);
        }

        public ICategoryService Category { get; private set; }

        public IApplicationUserService ApplicationUser { get; private set; }

        public IOrderDetailsService OrderDetails { get; private set; }

        public IOrderService Orders { get; private set; }

        public IProductService Product { get; private set; }

        public IShoppingCartService ShoppingCart { get; private set; }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}
