using DAL.Context;
using DAL.Models;
using DAL.Repositories.Interfaces;

namespace DAL.Repositories.Implementations
{
    public class AdminLogRepository : Repository<AdminLog>, IAdminLogRepository
    {
        public AdminLogRepository(AppDbContext context) : base(context)
        {
        }
    }
}
