using System.ComponentModel.DataAnnotations;

namespace backend.Models.Entity;

public class ProfileModel
{
    [Key]
    [Required]
    public string Email { get; set; }

    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
}