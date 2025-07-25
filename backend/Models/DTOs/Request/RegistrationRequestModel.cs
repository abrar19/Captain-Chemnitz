using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTOs;

public class RegistrationRequestModel
{
    
    [Required]
    public string FirstName { get; set; }
    
    [Required]
    public string LastName { get; set; }
    
    [Required]
    public string Email { get; set; }
    
    [Required]
    public string Password { get; set; }
}