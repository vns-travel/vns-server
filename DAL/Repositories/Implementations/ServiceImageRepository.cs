using DAL.Context;
using DAL.Models;
using DAL.Repositories.Interfaces;

namespace DAL.Repositories.Implementations
{
    public class ServiceImageRepository : Repository<ServiceImage>, IServiceImageRepository
    {
        public ServiceImageRepository(AppDbContext context) : base(context)
        {
        }
    }
}
