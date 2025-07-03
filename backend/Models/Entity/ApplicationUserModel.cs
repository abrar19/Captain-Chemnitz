using Microsoft.AspNetCore.Identity;

namespace backend.Models.Entity;

public class ApplicationUserModel : IdentityUser
{
    public ApplicationUserModel()
    {
        IsActive = true;
    }

    

    public bool IsActive { get; set;  }
}