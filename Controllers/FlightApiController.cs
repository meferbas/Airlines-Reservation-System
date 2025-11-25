using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AirlineSeatReservationSystem.Data;
using AirlineSeatReservationSystem.Data.Concrete.Efcore;
using AirlineSeatReservationSystem.Data.Abstract;
using AirlineSeatReservationSystem.Entity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using AirlineSeatReservationSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AirlineSeatReservationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FlightApiController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;


        private readonly IFlightRepository _repository;

        public FlightApiController(IFlightRepository repository, IConfiguration configuration, IUserRepository usersRepository)
        {
            _repository = repository;
            _userRepository = usersRepository;

            _configuration = configuration;

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] Flight model)
        {
            // Model doğrulaması yapılıyor.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Güncellenecek uçuşun varlığını kontrol ediliyor.
            var flightToUpdate = await _repository.Flights.FirstOrDefaultAsync(f => f.FlightId == id);
            if (flightToUpdate == null)
            {
                return NotFound();
            }

            // Gelen verileri var olan uçuşa atıyoruz.
            flightToUpdate.From = model.From;
            flightToUpdate.To = model.To;
            flightToUpdate.Depart = model.Depart;
            flightToUpdate.Return = model.Return;
            flightToUpdate.Time = model.Time;
            flightToUpdate.Guest = model.Guest;

            // Repository üzerinden güncelleme işlemi yapılıyor.
            _repository.editFlight(flightToUpdate);

            // İşlem başarılı olduğunda HTTP 204 No Content dönülüyor.
            return NoContent();
        }


        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(int id)
        {
            var flightToDelete = await _repository.Flights.FirstOrDefaultAsync(f => f.FlightId == id);
            if (flightToDelete == null)
            {
                return NotFound();
            }

            await _repository.DeleteFlight(flightToDelete);
            return NoContent();
        }
        [AllowAnonymous]
        [HttpPost("GenerateToken")]
        public async Task<IActionResult> GenerateToken([FromBody] SignInViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepository.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
                if (user != null && _userRepository.VerifyPassword(model.Password, user.Password))
                {
                    // Admin kontrolü
                    var isAdmin = (user.Email == "g211210013@sakarya.edu.tr" || user.Email == "g201210093@sakarya.edu.tr");

                    // Claims listesi oluşturuluyor
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserNo.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, isAdmin ? "admin" : "user")
            };

                    // JWT token için gerekli anahtar ve credentials
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var expiry = DateTime.Now.AddDays(Convert.ToInt32(_configuration["Jwt:ExpiryInDays"]));

                    // JWT token oluşturuluyor
                    var token = new JwtSecurityToken(
                        claims: claims,
                        expires: expiry,
                        signingCredentials: creds
                    );

                    // Token ve rol bilgisini içeren bir nesne döndürülüyor
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        role = isAdmin ? "admin" : "user"
                    });
                }
                else
                {
                    return Unauthorized();
                }
            }

            return BadRequest("Could not create token");
        }


    }
}
