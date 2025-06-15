using LoginApi.Models;

namespace LoginApi.Services
{
    public interface IUserService
    {
        bool Register(string username, string password);
        AuthResponse? Authenticate(string username, string password);
        string HashPassword(string password);
    }
}
