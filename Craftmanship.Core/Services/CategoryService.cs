using Craftmanship.Core.Data;
using Craftmanship.Core.Interfaces;
using Craftmanship.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Craftmanship.Core.Services
{
    public class CategoryService : Service<Categories>, ICategoryService
    {
        private AppDbContext _db;

        public CategoryService(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Categories category)
        {
            _db.Categories.Update(category);
        }
    }
}
