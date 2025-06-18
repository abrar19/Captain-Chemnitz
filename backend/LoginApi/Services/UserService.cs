using LoginApi.Models; 
using LoginApi.Data; 
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace LoginApi.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly string _jwtSecret;

        public UserService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _jwtSecret = configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key is not configured.");
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

            var token = GenerateJwtToken(user.Id, user.Username);

            return new AuthResponse
            {
                Token = token,
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

        public string GenerateJwtToken(int userId, string username)
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSecret); // Same as Program.cs
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
