using DAL.Context;
using DAL.Models;
using DAL.Repositories.Interfaces;

namespace DAL.Repositories.Implementations
{
    public class DestinationImageRepository : Repository<DestinationImage>, IDestinationImageRepository
    {
        public DestinationImageRepository(AppDbContext context) : base(context)
        {
        }
    }
}
