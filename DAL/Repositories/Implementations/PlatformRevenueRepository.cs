using DAL.Context;
using DAL.Models;
using DAL.Repositories.Interfaces;

namespace DAL.Repositories.Implementations
{
    public class PlatformRevenueRepository : Repository<PlatformRevenue>, IPlatformRevenueRepository
    {
        public PlatformRevenueRepository(AppDbContext context) : base(context)
        {
        }
    }
}
