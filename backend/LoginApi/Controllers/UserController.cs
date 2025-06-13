using LoginApi.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] UserRegisterRequest request)
    {
        var success = _userService.Register(request.Username, request.Password);
        if (!success)
        {
            return BadRequest("User already exists.");
        }
        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLoginRequest request)
    {
        var token = _userService.Authenticate(request.Username, request.Password);
        if (token == null)
        {
            return Unauthorized("Invalid username or password.");
        }
        return Ok(new { Token = token });
    }
}

public class UserRegisterRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class UserLoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}
