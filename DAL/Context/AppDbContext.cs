using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Context
{
    public class AppDbContext : DbContext
    {
        // Existing DbSets
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Voucher> Vouchers { get; set; }
        public virtual DbSet<Partner> Partners { get; set; }
        public virtual DbSet<PartnerDocument> PartnerDocuments { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Combo> Combos { get; set; }
        public virtual DbSet<ServicePromotion> ServicePromotions { get; set; }
        public virtual DbSet<Revenue> Revenues { get; set; }
        public virtual DbSet<FinancialReport> FinancialReports { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public DbSet<OtpCode> OtpCodes { get; set; }

        // New DbSets for missing models
        public virtual DbSet<HomestayService> HomestayServices { get; set; }
        public virtual DbSet<TourService> TourServices { get; set; }
        public virtual DbSet<VehicleRentalService> VehicleRentalServices { get; set; }
        public virtual DbSet<ComboItem> ComboItems { get; set; }
        public virtual DbSet<ServiceFeedback> ServiceFeedbacks { get; set; }
        public virtual DbSet<ServiceRating> ServiceRatings { get; set; }
        public virtual DbSet<HomestayRoom> HomestayRooms { get; set; }
        public virtual DbSet<HomestayBooking> HomestayBookings { get; set; }
        public virtual DbSet<HomestayAvailability> HomestayAvailabilities { get; set; }
        public virtual DbSet<TourBooking> TourBookings { get; set; }
        public virtual DbSet<TourSchedule> TourSchedules { get; set; }
        public virtual DbSet<TourItinerary> TourItineraries { get; set; }
        public virtual DbSet<VehicleRentalBooking> VehicleRentalBookings { get; set; }
        public virtual DbSet<Vehicle> Vehicles { get; set; }
        public virtual DbSet<UserBankAccount> UserBankAccounts { get; set; }
        public virtual DbSet<PartnerLocation> PartnerLocations { get; set; }
        public virtual DbSet<Refund> Refunds { get; set; }
        public virtual DbSet<SavedLocation> SavedLocations { get; set; }
        public virtual DbSet<AuthProvider> AuthProviders { get; set; }
        public virtual DbSet<Destination> Destinations { get; set; }
        public virtual DbSet<DestinationImage> DestinationImages { get; set; }
        public virtual DbSet<ServiceImage> ServiceImages { get; set; }
        public virtual DbSet<BookingItem> BookingItems { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Conversation> Conversations { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<AdminLog> AdminLogs { get; set; }
        public virtual DbSet<PlatformRevenue> PlatformRevenues { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships

            // Service relationships
            modelBuilder.Entity<Service>()
                .HasMany(s => s.ServicePromotions)
                .WithOne(sp => sp.Service)
                .HasForeignKey(sp => sp.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Service>()
                .HasOne(s => s.HomestayService)
                .WithOne(hs => hs.Service)
                .HasForeignKey<HomestayService>(hs => hs.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Service>()
                .HasOne(s => s.TourService)
                .WithOne(ts => ts.Service)
                .HasForeignKey<TourService>(ts => ts.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Service>()
                .HasOne(s => s.VehicleRentalService)
                .WithOne(vrs => vrs.Service)
                .HasForeignKey<VehicleRentalService>(vrs => vrs.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Service>()
                .HasMany(s => s.ServiceFeedbacks)
                .WithOne(sf => sf.Service)
                .HasForeignKey(sf => sf.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Service>()
                .HasMany(s => s.ServiceRatings)
                .WithOne(sr => sr.Service)
                .HasForeignKey(sr => sr.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Service>()
                .HasMany(s => s.ServiceImages)
                .WithOne(si => si.Service)
                .HasForeignKey(si => si.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Service>()
                .HasOne(s => s.Destination)
                .WithMany(d => d.Services)
                .HasForeignKey(s => s.DestinationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Destination>()
                .HasMany(d => d.DestinationImages)
                .WithOne(di => di.Destination)
                .HasForeignKey(di => di.DestinationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Combo relationships
            modelBuilder.Entity<Combo>()
                .HasMany(c => c.ComboItems)
                .WithOne(ci => ci.Combo)
                .HasForeignKey(ci => ci.ComboId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ComboItem>()
                .HasOne(ci => ci.Service)
                .WithMany(s => s.ComboItems)
                .HasForeignKey(ci => ci.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Homestay relationships
            modelBuilder.Entity<HomestayService>()
                .HasMany(hs => hs.HomestayRooms)
                .WithOne(hr => hr.HomestayService)
                .HasForeignKey(hr => hr.HomestayId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HomestayService>()
                .HasMany(hs => hs.HomestayAvailabilities)
                .WithOne(ha => ha.HomestayService)
                .HasForeignKey(ha => ha.HomestayId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HomestayService>()
                .HasMany(hs => hs.HomestayBookings)
                .WithOne(hb => hb.HomestayService)
                .HasForeignKey(hb => hb.HomestayId)
                .OnDelete(DeleteBehavior.Restrict);

            // Tour relationships
            modelBuilder.Entity<TourService>()
                .HasMany(ts => ts.TourSchedules)
                .WithOne(tch => tch.TourService)
                .HasForeignKey(tch => tch.TourId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TourService>()
                .HasMany(ts => ts.TourItineraries)
                .WithOne(ti => ti.TourService)
                .HasForeignKey(ti => ti.TourId)
                .OnDelete(DeleteBehavior.Restrict);

            // User relationships
            modelBuilder.Entity<User>()
                .HasMany(u => u.BankAccounts)
                .WithOne(uba => uba.User)
                .HasForeignKey(uba => uba.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.SavedLocations)
                .WithOne(sl => sl.User)
                .HasForeignKey(sl => sl.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Partner relationships
            modelBuilder.Entity<Partner>()
                .HasMany(p => p.PartnerLocations)
                .WithOne(pl => pl.Partner)
                .HasForeignKey(pl => pl.PartnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Composite key for PartnerLocation
            modelBuilder.Entity<PartnerLocation>()
                .HasKey(pl => new { pl.PartnerId, pl.LocationId });

            // Location relationships
            modelBuilder.Entity<Location>()
                .HasMany(l => l.ServiceRatings)
                .WithOne(sr => sr.Location)
                .HasForeignKey(sr => sr.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking relationships
            modelBuilder.Entity<Booking>()
                .HasMany(b => b.ServiceFeedbacks)
                .WithOne(sf => sf.Booking)
                .HasForeignKey(sf => sf.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasMany(b => b.BookingItems)
                .WithOne(bi => bi.Booking)
                .HasForeignKey(bi => bi.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasMany(b => b.Reviews)
                .WithOne(r => r.Booking)
                .HasForeignKey(r => r.BookingId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BookingItem>()
                .HasOne(bi => bi.Service)
                .WithMany(s => s.BookingItems)
                .HasForeignKey(bi => bi.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking-Payment relationship - prevent cascade path cycles
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.SetNull);

            // Refund relationships - prevent cascade path cycles
            modelBuilder.Entity<Refund>()
                .HasOne(r => r.Booking)
                .WithMany(b => b.Refunds)
                .HasForeignKey(r => r.BookingId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Refund>()
                .HasOne(r => r.Payment)
                .WithMany(p => p.Refunds)
                .HasForeignKey(r => r.PaymentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Service)
                .WithMany(s => s.Reviews)
                .HasForeignKey(r => r.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.BookingItem)
                .WithMany()
                .HasForeignKey(r => r.BookingItemId)
                .OnDelete(DeleteBehavior.SetNull);

            // ServiceFeedback relationships - prevent cascade path cycles
            modelBuilder.Entity<ServiceFeedback>()
                .HasOne(sf => sf.User)
                .WithMany(u => u.ServiceFeedbacks)
                .HasForeignKey(sf => sf.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ServiceFeedback>()
                .HasOne(sf => sf.Partner)
                .WithMany()
                .HasForeignKey(sf => sf.PartnerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ServiceFeedback>()
                .HasOne(sf => sf.Booking)
                .WithMany(b => b.ServiceFeedbacks)
                .HasForeignKey(sf => sf.BookingId)
                .OnDelete(DeleteBehavior.NoAction);

            // ServiceFeedback -> Service already configured above via Service.ServiceFeedbacks

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AuthProvider>()
                .HasOne(ap => ap.User)
                .WithMany(u => u.AuthProviders)
                .HasForeignKey(ap => ap.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Destination>()
                .Property(d => d.Latitude)
                .HasPrecision(9, 6);

            modelBuilder.Entity<Destination>()
                .Property(d => d.Longitude)
                .HasPrecision(9, 6);

        }
    }
}
