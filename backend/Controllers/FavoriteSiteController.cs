using backend.Data;
using backend.Models.DTOs;
using backend.Models.DTOs.Response;
using backend.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;


[ApiController]
[Route("[controller]")]
public class FavoriteSiteController: ControllerBase
{
    
    private readonly APIDbContext _apiDbContext;
    private UserManager<ApplicationUserModel> _userManager;
    
    public FavoriteSiteController(APIDbContext apiDbContext, UserManager<ApplicationUserModel> userManager)
    {
        _apiDbContext = apiDbContext;
        _userManager = userManager;
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
        
        var favSite= await _apiDbContext.favoriteSites.Where(fs => fs.CulturalSiteId == model.CulturalSiteId && fs.profileModel.Email == user.Email).FirstOrDefaultAsync();
        
        if (favSite != null)
        {
            return BadRequest(new ErrorReponseModel
            {
                error = "Already Exists",
                message = "This cultural site is already in your favorites."
            });
        }
        else
        {
            FavoriteSiteModel favoriteSite = new FavoriteSiteModel
            {
                CulturalSiteId = model.CulturalSiteId,
                culturalSiteModel = culturalSite,
                profileModel = profile,
                Email = user.Email
            };
            _apiDbContext.favoriteSites.Add(favoriteSite);
            var result= await _apiDbContext.SaveChangesAsync();

            if (result > 0)
            {
                return Ok(new { message = "Cultural site added to favorites successfully." });
            }
            else
            {
             return  BadRequest(new ErrorReponseModel
                {
                    error = "Error",
                    message = "Failed to add cultural site to favorites."
                });
            }
            
        }
        
        // FavoriteSiteModel favoriteSite = new FavoriteSiteModel
        // {
        //     CulturalSiteId = model.CulturalSiteId,
        //     user = profile,
        //     Email = user.Email
        // };
        //
        // profile.FavoriteSites.Add(favoriteSite);
        // _apiDbContext.favoriteSites.Add(favoriteSite);
        // await _apiDbContext.SaveChangesAsync();

        return Ok(new { message = "Cultural site added to favorites successfully." });
    }

    
    [HttpGet()]
    [Route("GetFavoriteSites")]
    [Authorize(Roles = "AppUsers")]
    public async Task<IActionResult> GetFavoriteSites()
    {
        string userId = User.Claims.First(c => c.Type == "UserID").Value;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Unauthorized(new ErrorReponseModel
            {
                error = "Unauthorized",
                message = "You must be logged in to view favorite sites."
            });
        }

        ProfileModel profile = await _apiDbContext.profiles.FirstOrDefaultAsync(x => x.Email == user.Email);
        if (profile == null)
        {
            return NotFound(new ErrorReponseModel
            {
                error = "Not Found",
                message = "Profile not found."
            });
        }

        var favoriteSites = await _apiDbContext.favoriteSites
            .Where(fs => fs.profileModel.Email == user.Email)
            .Select(fs=> new FavoriteSiteResponseModel(fs.FavoriteSiteId, new CultureSiteResponseModel(fs.culturalSiteModel)))
            .ToListAsync();

        return Ok(favoriteSites);
    }
    
    [HttpDelete("removeFromFavorites/{favoriteSiteId}")]
    [Authorize(Roles = "AppUsers")]
    public async Task<IActionResult> RemoveFromFavorites(int favoriteSiteId)
    {
        string userId = User.Claims.First(c => c.Type == "UserID").Value;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Unauthorized(new ErrorReponseModel
            {
                error = "Unauthorized",
                message = "You must be logged in to remove favorites."
            });
        }
        
        var favoriteSite = await _apiDbContext.favoriteSites
            .Where(fs => fs.FavoriteSiteId == favoriteSiteId && fs.profileModel.Email == user.Email)
            .FirstOrDefaultAsync();
      
        if (favoriteSite == null)
        {
            return NotFound(new ErrorReponseModel
            {
                error = "Not Found",
                message = "Favorite site not found."
            });
        }
        else
        {
            var result = await _apiDbContext.favoriteSites
                .Where(fs => fs.FavoriteSiteId == favoriteSiteId && fs.profileModel.Email == user.Email)
                .ExecuteDeleteAsync();
            if (result == 0)
            {
                return BadRequest(new ErrorReponseModel
                {
                    error = "Error",
                    message = "Failed to remove cultural site from favorites."
                });
            }else
            {
                await _apiDbContext.SaveChangesAsync();
                
                return Ok(new { message = "Cultural site removed from favorites successfully." });
            }
        }
        
    }
}