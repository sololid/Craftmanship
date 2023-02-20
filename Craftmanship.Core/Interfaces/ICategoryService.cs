using Craftmanship.Core.Models;

namespace Craftmanship.Core.Interfaces
{
    public interface ICategoryService : IService<Categories>
    {
        void Update(Categories category);
    }
}
