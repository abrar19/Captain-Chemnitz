using LoginApi.Data;
using LoginApi.Models;
using LoginApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly AppDbContext _context;

    public UserController(IUserService userService, AppDbContext context)
    {
        _userService = userService;
         _context = context;
    }

    private bool IsAuthorized()
    {
        return Request.Headers.TryGetValue("Authorization", out var authHeader) &&
            authHeader == "Bearer dummy-token";
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
        var authResponse = _userService.Authenticate(request.Username, request.Password);
        if (authResponse == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        return Ok(authResponse);
    }

    [HttpPost("{userId}/favorites")]
    public async Task<IActionResult> AddFavorite(int userId, [FromBody] FavoritePlaceDto favoriteDto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return NotFound("User not found.");

        // Check for existing favorite with same name
        var exists = await _context.FavoritePlaces
            .AnyAsync(f => f.UserId == userId && f.Name == favoriteDto.Name);

        if (exists)
            return Conflict("This place is already in your favorites.");

        var favorite = new FavoritePlace
        {
            Name = favoriteDto.Name,
            Description = favoriteDto.Description,
            UserId = userId
        };

        _context.FavoritePlaces.Add(favorite);
        await _context.SaveChangesAsync();

         return Ok(new FavoritePlaceDto
        {
            Id = favorite.Id,
            Name = favorite.Name,
            Description = favorite.Description
        });
    }

    [HttpGet("{userId}/favorites")]
    public async Task<IActionResult> GetFavorites(int userId)
    {
        var favorites = await _context.FavoritePlaces
            .Where(f => f.UserId == userId)
            .ToListAsync();

        return Ok(favorites);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound("User not found.");

        return Ok(new { user.Id, user.Username });
    }


    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateProfile(int userId, [FromBody] UserUpdateRequest request)
    {   
        //
        if (!IsAuthorized())
            return Unauthorized("Invalid or missing token.");

        // Find the user
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return NotFound("User not found.");

        // Update allowed fields
        user.Username = request.Username ?? user.Username;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.PasswordHash = _userService.HashPassword(request.Password); // Ensure this hashes the password
        }

        await _context.SaveChangesAsync();
        return Ok("Profile updated successfully.");
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

public class UserUpdateRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}
