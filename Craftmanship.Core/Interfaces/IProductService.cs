using Craftmanship.Core.Models;

namespace Craftmanship.Core.Interfaces
{
    public interface IProductService : IService<Products>
    {
        void Update(Products product);
    }
}