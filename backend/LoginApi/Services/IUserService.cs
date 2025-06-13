namespace LoginApi.Services
{
    public interface IUserService
    {
        bool Register(string username, string password);
        string? Authenticate(string username, string password);
    }
}
