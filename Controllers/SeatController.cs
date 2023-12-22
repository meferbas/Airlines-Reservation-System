using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AirlineSeatReservationSystem.Data;
using AirlineSeatReservationSystem.Data.Concrete.Efcore;
using AirlineSeatReservationSystem.Data.Abstract;
using AirlineSeatReservationSystem.Entity;

using AirlineSeatReservationSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;


namespace AirlineSeatReservationSystem.Controllers
{
    public class SeatController : Controller
    {
        private readonly ISeatRepository _seatRepository;
        private readonly IBookingRepository _bookingRepository;
        private IFlightRepository _repository;

        private readonly DataContext _context;
        public SeatController(ISeatRepository seatRepository, DataContext context, IFlightRepository repository, IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
            _seatRepository = seatRepository;
            _context = context;
            _repository = repository;
        }

        public IActionResult ChooseSeats(int flightId)
        {
            var flightExists = _context.Flights.Any(f => f.FlightId == flightId);
            if (!flightExists)
            {
                // Uçuş yoksa, uygun bir hata mesajı gönderin.
                return NotFound(flightId);
            }

            var seats = _context.Seats.Where(s => s.FlightId == flightId).ToList();
            // Eğer hiç koltuk yoksa, varsayılan olarak 20 koltuk oluşturun ve veritabanına ekleyin
            if (!seats.Any())
            {
                for (int i = 1; i <= 20; i++)
                {
                    seats.Add(new Seat { FlightId = flightId, SeatNumber = $"Seat {i}", IsOccupied = false });
                }
                _context.AddRange(seats);
                _context.SaveChanges();
            }

            var model = new ChooseSeatsViewModel
            {
                FlightId = flightId,
                Seats = seats.Select(s => new SeatModel
                {
                    SeatId = s.SeatId,
                    SeatNumber = s.SeatNumber, // Artık dönüştürmeye gerek yok
                    IsOccupied = s.IsOccupied
                }).ToList()
            };


            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> ChooseSeats(ChooseSeatsViewModel model, int flightId)
        {
            await _seatRepository.ReserveSeat(model.SelectedSeat);
            TempData["SuccessMessage"] = "Uçuşunuz başarılı bir şekilde oluşturuldu.";

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                var userNo = int.Parse(userIdClaim.Value);
                var booking = new Booking
                {
                    UserNo = userNo,
                    FlightId = flightId,
                    SeatId = model.SelectedSeat,

                };

                _bookingRepository.Add(booking);
                _bookingRepository.SaveChanges(); // Eğer kaydetme işlemi async ise

                return RedirectToAction("Index", "Flight");
            }
            else
            {
                return Json(new { success = false, message = "User not authenticated." });
            }
        }



    }
}
