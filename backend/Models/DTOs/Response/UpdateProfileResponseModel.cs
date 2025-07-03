using backend.Models.Entity;

namespace backend.Models.DTOs.Response;

public class UpdateProfileResponseModel
{
    public UpdateProfileResponseModel(ProfileModel profile)
    {
        Email = profile.Email;
        FirstName = profile.FirstName;
        LastName = profile.LastName;
    }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}