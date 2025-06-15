using LoginApi.Models; 
using LoginApi.Data; 
using System.Security.Cryptography;
using System.Text;

namespace LoginApi.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool Register(string username, string password)
        {
            if (_context.Users.Any(u => u.Username == username))
                return false;

            var hashed = HashPassword(password);

            var newUser = new User
            {
                Username = username,
                PasswordHash = hashed
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return true;
        }

        public AuthResponse? Authenticate(string username, string password)
        {
            var passwordHash = ComputeSha256Hash(password);

            var user = _context.Users.SingleOrDefault(u => u.Username == username && u.PasswordHash == passwordHash);

            if (user == null)
                return null;

            return new AuthResponse
            {
                Token = "dummy-token",
                UserId = user.Id
            };
        }

        private static string ComputeSha256Hash(string rawData)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(rawData);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash); 
        }


        public User? GetByUsername(string username)
        {
            return _context.Users.SingleOrDefault(u => u.Username == username);
        }

    }
}
