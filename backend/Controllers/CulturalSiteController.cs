using backend.Models.Entity;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using backend.Data;
using backend.Models.DTOs;
using backend.Models.DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Linq;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace backend.Controllers;
[ApiController]
[Route("[controller]")]
public class CulturalSiteController: ControllerBase
{
    private readonly APIDbContext _apiDbContext;
    private UserManager<ApplicationUserModel> _userManager;
    
    public CulturalSiteController(APIDbContext apiDbContext, UserManager<ApplicationUserModel> userManager)
    {
        _apiDbContext = apiDbContext;
        _userManager = userManager;
    }
    
    [HttpGet]
    [Route("GetCulturalSites")]
    public async Task<IActionResult> GetCulturalSites()
    {
        var culturalSites = _apiDbContext.culturalSites.Select(model => new CultureSiteResponseModel(model));

        return Ok(culturalSites.ToList());
    }
    
    
    [HttpGet("getCulturalSiteWithin10Min")]
    public async Task<IActionResult> GetSitesWithin10Min(double lat, double lng)
    {
        var center = new Point(lng, lat) { SRID = 4326 };

        // Approx 800 meters = 10 min walking at 5 km/h
        double radiusInMeters = 800;
        var sites = await _apiDbContext.culturalSites
            .Where(site => site.Location.IsWithinDistance(center, radiusInMeters))
            .Select(site =>new CultureSiteResponseModel(site) )
            .ToListAsync();

       

        return Ok(sites.ToList());
    }
    
    
    //add to favorites
    [HttpPost("addToFavorites")]
    [Authorize(Roles = "AppUsers")]
    public async Task<IActionResult> AddToFavorites(AddFavoriteRequestModel model)
    {
        string userId = User.Claims.First(c => c.Type == "UserID").Value;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Unauthorized(new ErrorReponseModel
            {
                error = "Unauthorized",
                message = "You must be logged in to add favorites."
            });
        }

        var culturalSite = await _apiDbContext.culturalSites.FindAsync(model.CulturalSiteId);
        if (culturalSite == null)
        {
            return NotFound(new ErrorReponseModel
            {
                error = "Not Found",
                message = "Cultural site not found."
            });
        }

        ProfileModel profile = await _apiDbContext.profiles.FirstOrDefaultAsync(x => x.Email == user.Email);
        // Check if the site is already in favorites
        if (profile.FavoriteSites.Any(fs => fs.CulturalSiteId == model.CulturalSiteId))
        {
            return BadRequest(new ErrorReponseModel
            {
                error = "Already Exists",
                message = "This cultural site is already in your favorites."
            });
        }
        
        FavoriteSiteModel favoriteSite = new FavoriteSiteModel
        {
            CulturalSiteId = model.CulturalSiteId,
            user = profile,
            Email = user.Email
        };

        profile.FavoriteSites.Add(favoriteSite);
        _apiDbContext.favoriteSites.Add(favoriteSite);
        await _apiDbContext.SaveChangesAsync();

        return Ok(new { message = "Cultural site added to favorites successfully." });
    }




}