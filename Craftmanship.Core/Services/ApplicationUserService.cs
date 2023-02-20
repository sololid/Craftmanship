using Craftmanship.Core.Data;
using Craftmanship.Core.Interfaces;
using Craftmanship.Core.Models;

namespace Craftmanship.Core.Services
{
    public class ApplicationUserService : Service<ApplicationUser>, IApplicationUserService
    {
        private AppDbContext _db;
        public ApplicationUserService(AppDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
