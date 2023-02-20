using Craftmanship.Core.Data;
using Craftmanship.Core.Interfaces;
using Craftmanship.Core.Models;

namespace Craftmanship.Core.Services
{
    public class ProductService : Service<Products>, IProductService
    {
        private AppDbContext _db;

        public ProductService(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Products product)
        {
            var productFromDb = _db.Products.FirstOrDefault(x => x.Id == product.Id);
            if (productFromDb != null)
            {
                productFromDb.Name = product.Name;
                productFromDb.Description = product.Description;
                productFromDb.Price = product.Price;
                productFromDb.CategoryId = product.CategoryId;
                if (productFromDb.ImageUrl != null)
                {
                    productFromDb.ImageUrl = product.ImageUrl;
                } 
                //if (productFromDb.VendorId != 0)
                //{
                //    productFromDb.VendorId = product.VendorId;
                //}
            }
        }
    }
}
