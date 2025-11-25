using AirlineSeatReservationSystem.Entity;
using AirlineSeatReservationSystem.Data.Abstract;
using AirlineSeatReservationSystem.Data.Concrete.Efcore;
using System.Security.Cryptography;
using System.Text;

namespace AirlineSeatReservationSystem.Data.Concrete
{
    public class EfUserRepository : IUserRepository
    {

        private DataContext _context;
        public EfUserRepository(DataContext context)
        {
            _context = context;
        }
        public IQueryable<User> Users => _context.Users;
        public void CreateUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            return hash;
        }
        }

        public bool VerifyPassword(string inputPassword, string storedHash)
        {
            // Kullanıcıdan alınan şifreyi hash'leyin
            var hashedInputPassword = HashPassword(inputPassword);

            // Hash'lenmiş kullanıcı şifresi ile veritabanındaki hash'lenmiş şifreyi karşılaştırın
            return hashedInputPassword == storedHash;
        }
    }
}