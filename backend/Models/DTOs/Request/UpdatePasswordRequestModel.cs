namespace backend.Models.DTOs;

public class UpdatePasswordRequestModel
{
    public string OldPassword { get; set; }
    
    public string NewPassword { get; set; }
}