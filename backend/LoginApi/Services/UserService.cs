using LoginApi.Models; // your User model namespace
using LoginApi.Data;   // your AppDbContext namespace
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

        public bool Register(string username, string password)
        {
            if (_context.Users.Any(u => u.Username == username))
                return false;

            var passwordHash = ComputeSha256Hash(password);

            var newUser = new User
            {
                Username = username,
                PasswordHash = passwordHash
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return true;
        }

        public string? Authenticate(string username, string password)
        {
            var passwordHash = ComputeSha256Hash(password);

            var user = _context.Users.SingleOrDefault(u => u.Username == username && u.PasswordHash == passwordHash);

            if (user == null)
                return null;

            // TODO: return a real JWT token later
            return "dummy-token";
        }

        private static string ComputeSha256Hash(string rawData)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
    }
}
