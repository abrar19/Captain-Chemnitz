using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Data;
using backend.Models.DTOs;
using backend.Models.DTOs.Response;
using backend.Models.Entity;
using backend.Models.Settings;
using backend.Services;
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

    /// <summary>
    /// Can be access by Unauthorized users. This feature is used to verify email address
    /// </summary>
  /*  [HttpGet ("ConfirmEmailAsyncTest")]
    public async Task<Object> ConfirmEmailAsyncTest(string email, string code)
    {
        if (email == null || code == null)
        {
            return BadRequest(new { message = "Somting is missing" });
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return BadRequest(new { message = "User not found" });
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (result.Succeeded)
        {
            return Ok(new { Message = "Verification Succeeded" });
        }
        else
        {
            return Ok(new { Message = "Verification Failed" });
        }
    }*/
    
    
  
}