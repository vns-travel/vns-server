using DAL.Context;
using DAL.Models;
using DAL.Repositories.Interfaces;

namespace DAL.Repositories.Implementations
{
    public class BookingItemRepository : Repository<BookingItem>, IBookingItemRepository
    {
        public BookingItemRepository(AppDbContext context) : base(context)
        {
        }
    }
}
