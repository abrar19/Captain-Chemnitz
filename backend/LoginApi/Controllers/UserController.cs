using LoginApi.Data;
using LoginApi.Models;
using LoginApi.Services;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize]
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
            LocationId = favoriteDto.LocationId,
            UserId = userId
        };

        _context.FavoritePlaces.Add(favorite);
        await _context.SaveChangesAsync();

         return Ok(new FavoritePlaceDto
        {
            Id = favorite.Id,
            Name = favorite.Name,
            LocationId = favorite.LocationId
        });
    }

    [Authorize]
    [HttpGet("{userId}/favorites")]
    public async Task<IActionResult> GetFavorites(int userId)
    {
        var favorites = await _context.FavoritePlaces
            .Where(f => f.UserId == userId)
            .ToListAsync();

        return Ok(favorites);
    }

    [Authorize]
    [HttpDelete("{userId}/favorites/{favoriteId}")]
    public async Task<IActionResult> RemoveFavorite(int userId, int favoriteId)
    {
        var favorite = await _context.FavoritePlaces
            .FirstOrDefaultAsync(f => f.UserId == userId && f.Id == favoriteId);

        if (favorite == null)
            return NotFound("Favorite not found.");

        _context.FavoritePlaces.Remove(favorite);
        await _context.SaveChangesAsync();

        return NoContent(); // 204 success
    }

    [Authorize]
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound("User not found.");

        return Ok(new { user.Id, user.Username });
    }

    [Authorize]
    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateProfile(int userId, [FromBody] UserUpdateRequest request)
    {   
        // Find the user
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return NotFound("User not found.");

        // Update allowed fields
        user.Username = request.Username ?? user.Username;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.PasswordHash = _userService.HashPassword(request.Password); //this hashes the password
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
