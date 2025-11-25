    using AirlineSeatReservationSystem.Entity;

    public class MyBookingsViewModel
    {
        public IEnumerable<Booking> Bookings { get; set; } = new List<Booking>();
        // Diğer gerekli alanlar...
    }