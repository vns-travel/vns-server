using DAL.Context;
using DAL.Models;
using DAL.Repositories.Interfaces;

namespace DAL.Repositories.Implementations
{
    public class OtpCodeRepository : Repository<OtpCode>, IOtpCodeRepository
    {
        public OtpCodeRepository(AppDbContext context) : base(context)
        {
        }
    }
}
