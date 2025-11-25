using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirlineSeatReservationSystem.Entity;

public class Booking
{
    [Key]
    public int BookingId { get; set; }
    public int UserNo { get; set; }

    public int FlightId { get; set; }
    public int SeatId { get; set; }


    public virtual Flight Flight { get; set; } = null!;
    public virtual Seat Seat { get; set; } = null!;
}
