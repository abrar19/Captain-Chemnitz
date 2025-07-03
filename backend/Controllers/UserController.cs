using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Data;
using backend.Models.DTOs;
using backend.Models.DTOs.Response;
using backend.Models.Entity;
using backend.Models.Settings;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private UserManager<ApplicationUserModel> _userManager;
    private SignInManager<ApplicationUserModel> _singInManager;
    private readonly ApplicationSettings _appSettings;
    private readonly APIDbContext _context;

    public UserController(UserManager<ApplicationUserModel> userManager,
        SignInManager<ApplicationUserModel> signInManager, IOptions<ApplicationSettings> appSettings,
        APIDbContext context)
    {
        _userManager = userManager;
        _singInManager = signInManager;
        _appSettings = appSettings.Value;
        _context = context;
    }

    /// <summary>
    /// Can be access by Unauthorize users
    /// </summary>
    /// 
    [HttpPost]
    [Route("Registration")]
    [ProducesResponseType(statusCode:StatusCodes.Status200OK, Type = typeof(RegistrationResponseModel))]
    [ProducesResponseType(statusCode:StatusCodes.Status400BadRequest, Type = typeof(ErrorReponseModel))]
    public async Task<Object> UserRegistration(RegistrationRequestModel model)
    {
        var usr = await _userManager.FindByEmailAsync(model.Email);
        if (usr != null)
        {
            return BadRequest(new ErrorReponseModel
            {
                error = "Email Already Exists",
                message = "The email address you entered is already associated with an account."
            });
        }

        var Role = "AppUsers";
        var applicationUser = new ApplicationUserModel()
        {
            UserName = model.Email,
            Email = model.Email
        };

        ProfileModel profile = new ProfileModel()
        {
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
        };

        try
        {
            var result = await _userManager.CreateAsync(applicationUser, model.Password);
            await _userManager.AddToRoleAsync(applicationUser, Role);

            _context.profiles.Add(profile);
            await _context.SaveChangesAsync();

            if (result.Succeeded)
            {

                try
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
                    var callbackUrl = Url.Action(("ConfirmEmail"), "User", new { userId = applicationUser.Id, code = code }, Request.Scheme);

                    EmailSender emailSender = new EmailSender();
                    emailSender.sendVerificationEmail(model.Email, callbackUrl, _appSettings);   
                }catch (Exception ex)
                {
                    
                }
                
                return Ok(new RegistrationResponseModel
                {
                    message = "Registration Successful. Please check your email to confirm your account.",
                });
            }
            else
            {
                return BadRequest(new ErrorReponseModel
                    {
                        error = "Registration Failed",
                        message = "There was an error during registration. Please try again."
                    }
                    );
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorReponseModel
                {
                    error = "Registration Failed",
                    message = "There was an error during registration. Please try again."
                }
            );
        }
    }


    /// <summary>
    /// Can be access by Unauthorizeduser
    /// </summary>
    /// <response code="403">Please confirm your email address before logging in</response>
    [HttpPost]
    [Route("Login")]
    [ProducesResponseType(statusCode:StatusCodes.Status200OK, Type = typeof(LoginResponseModel))]
    [ProducesResponseType(statusCode:StatusCodes.Status400BadRequest, Type = typeof(ErrorReponseModel))]
    public async Task<IActionResult> Login(LoginRequestModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
                
            if (! await _userManager.IsEmailConfirmedAsync(user))
            {
                return BadRequest( new ErrorReponseModel { error = "Email Not Confirmed" ,
                    message = "Please confirm your email address before logging in." });
            }
            if(!user.IsActive)
            {
                return BadRequest(new ErrorReponseModel
                {
                    error = "Account Deactivated",
                    message = "Your account has been deactivated. Please contact support."
                });
            }
            
            var role = await _userManager.GetRolesAsync(user);
            IdentityOptions _options = new IdentityOptions();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("UserID",user.Id.ToString()),
                    new Claim(_options.ClaimsIdentity.RoleClaimType,role.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);
            
            ProfileModel profile = await _context.profiles.FirstOrDefaultAsync(x => x.Email == model.Email) ?? new ProfileModel();
            LoginResponseModel response = new LoginResponseModel
            {
                Email = user.Email,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Token = token,
                Role = role.FirstOrDefault()
            };
            return Ok(response);
        }
        else
            return BadRequest(new ErrorReponseModel
            {
                error = "Invalid Credentials",
                message = "The email or password you entered is incorrect."
            });
    }

    /// <summary>
    /// Can be access by Unauthorized users. This feature is used to verify email address
    /// </summary>
    [HttpGet]
    public async Task<Object> ConfirmEmailAsync(string userId, string code)
    {
        if (userId == null || code == null)
        {
            return BadRequest(new { message = "Somting is missing" });
        }

        var user = await _userManager.FindByIdAsync(userId);
        var result = await _userManager.ConfirmEmailAsync(user, code);

        if (result.Succeeded)
        {
            return Ok(new { Message = "Verification Succeeded" });
        }
        else
        {
            return Ok(new { Message = "Verification Failed" });
        }
    }

  
    //user password update with old password and new password
    [HttpPost]
    [Route("UpdatePassword")]
    [ProducesResponseType(statusCode:StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(statusCode:StatusCodes.Status400BadRequest, Type = typeof(ErrorReponseModel))]
    [Authorize(Roles = "AppUsers")]
    public async Task<IActionResult> UpdatePassword(UpdatePasswordRequestModel model)
    {
        
        
        var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
        if (userId == null)
        {
            return Unauthorized(new ErrorReponseModel
            {
                error = "Unauthorized",
                message = "You must be logged in to update your password."
            });
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new ErrorReponseModel
            {
                error = "User Not Found",
                message = "The user does not exist."
            });
        }

        var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (result.Succeeded)
        {
            return Ok("Password updated successfully.");
        }
        
        return BadRequest(new ErrorReponseModel
        {
            error = "Update Failed",
            message = "There was an error updating the password. Please try again."
        });
    }
    
    
    //user profile update
    [HttpPut]
    [Route("UpdateProfile")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(ProfileModel))]
    [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest, Type = typeof(ErrorReponseModel))]
    [Authorize(Roles = "AppUsers")]
    public async Task<IActionResult> UpdateProfile(ProfileUpdateRequestModel model)
    {
        if (model == null)
        {
            return BadRequest(new ErrorReponseModel
            {
                error = "Invalid Data",
                message = "Profile update data is missing or invalid."
            });
        }else if (string.IsNullOrEmpty(model.FirstName) && string.IsNullOrEmpty(model.LastName) )
        {
            return BadRequest(new ErrorReponseModel
            {
                error = "No Data Provided",
                message = "At least one field (First Name, Last Name) must be provided to update the profile."
            });
        }
        else
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            if (userId == null)
            {
                return Unauthorized(new ErrorReponseModel
                {
                    error = "Unauthorized",
                    message = "You must be logged in to update your profile."
                });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new ErrorReponseModel
                {
                    error = "User Not Found",
                    message = "The user does not exist."
                });
            }

            ProfileModel profile = await _context.profiles.FirstOrDefaultAsync(x => x.Email == user.Email);
            if (profile == null)
            {
                return NotFound(new ErrorReponseModel
                {
                    error = "Profile Not Found",
                    message = "The profile does not exist."
                });
            }

            if (!string.IsNullOrEmpty(model.FirstName))
            {
                profile.FirstName = model.FirstName;
            }
            
            if (!string.IsNullOrEmpty(model.LastName))
            {
                profile.LastName = model.LastName;
            }

            var result=  _context.profiles.Update(profile);
            await _context.SaveChangesAsync();

            return Ok( new UpdateProfileResponseModel(profile));
        }
        
    }
    
    
    //soft delete user
    [HttpDelete]
    [Route("DeleteUser")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest, Type = typeof(ErrorReponseModel))]
    [Authorize(Roles = "AppUsers")]
    public async Task<IActionResult> DeleteUser()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
        if (userId == null)
        {
            return Unauthorized(new ErrorReponseModel
            {
                error = "Unauthorized",
                message = "You must be logged in to delete your account."
            });
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new ErrorReponseModel
            {
                error = "User Not Found",
                message = "The user does not exist."
            });
        }

        if (!user.IsActive)
        {
            return BadRequest(
                new ErrorReponseModel
                {
                    error = "Account Already Deleted",
                    message = "Your account has already been deleted."
                });
        }

        
        user.IsActive = false; 
        var result = await _userManager.UpdateAsync(user);
        
        if (result.Succeeded)
        {
            return Ok("User account deleted successfully.");
        }
        
        return BadRequest(new ErrorReponseModel
        {
            error = "Delete Failed",
            message = "There was an error deleting the user account. Please try again."
        });
        
        
    }
    
    //Get All Inactive Users
    [HttpGet]
    [Route("GetInactiveUsers")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(List<ApplicationUserModel>))]
    [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest, Type = typeof(ErrorReponseModel))]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetInactiveUsers()
    {
        var inactiveUsers = await _userManager.Users
            .Where(u => !u.IsActive)
            .Select(user=>new InActiveUserResponseModel
            {
                
                Email = user.Email,
                FirstName = _context.profiles.FirstOrDefault(p => p.Email == user.Email).FirstName,
                LastName = _context.profiles.FirstOrDefault(p => p.Email == user.Email).LastName,
            })
            .ToListAsync();

        if (inactiveUsers == null || inactiveUsers.Count == 0)
        {
            return NotFound(new ErrorReponseModel
            {
                error = "No Inactive Users",
                message = "There are no inactive users in the system."
            });
        }

        return Ok( inactiveUsers);
    }
    
    
    //get user profile by token
    [HttpGet]
    [Route("GetUserProfile")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(ProfileModel))]
    [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest, Type = typeof(ErrorReponseModel))]
    [Authorize(Roles = "AppUsers")]
    public async Task<IActionResult> GetUserProfile()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
        if (userId == null)
        {
            return Unauthorized(new ErrorReponseModel
            {
                error = "Unauthorized",
                message = "You must be logged in to view your profile."
            });
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new ErrorReponseModel
            {
                error = "User Not Found",
                message = "The user does not exist."
            });
        }

        if (!user.IsActive)
        {
            return BadRequest(new ErrorReponseModel
            {
                error = "Account Deactivated",
                message = "Your account has been deactivated. Please contact support."
            }); 
        }

        ProfileModel profile = await _context.profiles.FirstOrDefaultAsync(x => x.Email == user.Email);
        if (profile == null)
        {
            return NotFound(new ErrorReponseModel
            {
                error = "Profile Not Found",
                message = "The profile does not exist."
            });
        }

        return Ok(new UpdateProfileResponseModel(profile));
    }
    
    

    
  
}