using System.Runtime.Serialization;

namespace backend.Models.DTOs.Response;

public class LoginResponseModel
{
    
    public string Token { get; set; }
    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}