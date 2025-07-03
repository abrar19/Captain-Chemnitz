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
public class ReviewController : ControllerBase
{
    
    private readonly APIDbContext _apiDbContext;
    private readonly UserManager<ApplicationUserModel> _userManager;
    
    public ReviewController(APIDbContext apiDbContext, UserManager<ApplicationUserModel> userManager)
    {
        _apiDbContext = apiDbContext;
        _userManager = userManager;
    }
    
    [HttpPost]
    [Route("addReview")]
    [Authorize(Roles = "AppUsers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    
    public async Task<IActionResult> AddReview(AddReviewRequestModel reviewModel)
    {
        string userId = User.Claims.First(c => c.Type == "UserID").Value;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Unauthorized(new ErrorReponseModel
            {
                error = "Unauthorized",
                message = "You must be logged in to add a review."
            });
        }

        var culturalSite = await _apiDbContext.culturalSites.FindAsync(reviewModel.CulturalSiteId);
        if (culturalSite == null)
        {
            return NotFound(new ErrorReponseModel
            {
                error = "Not Found",
                message = "Cultural site not found."
            });
        }
        var profile = await _apiDbContext.profiles.FirstOrDefaultAsync(p => p.Email == user.Email);
        if (profile == null)
        {
            return NotFound(new ErrorReponseModel
            {
                error = "Not Found",
                message = "Profile not found."
            });
        }
        if (reviewModel.Rating < 1 || reviewModel.Rating > 5)
        {
            return BadRequest(new ErrorReponseModel
            {
                error = "Bad Request",
                message = "Rating must be between 1 and 5."
            });
        }
        if (string.IsNullOrWhiteSpace(reviewModel.ReviewText))
        {
            return BadRequest(new ErrorReponseModel
            {
                error = "Bad Request",
                message = "Review text cannot be empty."
            });
        }
        
        // if the user has already reviewed this cultural site
        var existingReview = await _apiDbContext.reviews
            .FirstOrDefaultAsync(r => r.CulturalSiteId == reviewModel.CulturalSiteId && r.Email == user.Email);
        if (existingReview != null)
        {
            return BadRequest(new ErrorReponseModel
            {
                error = "Already Exists",
                message = "You have already reviewed this cultural site."
            });
        }
        

    

        var review = new ReviewModel
        {   Email = user.Email,
            ProfileModel = profile,
            CulturalSiteId = reviewModel.CulturalSiteId,
            CulturalSiteModel = culturalSite,
            Rating = reviewModel.Rating,
            Text = reviewModel.ReviewText,
            CreatedAt = DateTime.UtcNow,
            
        };

        _apiDbContext.reviews.Add(review);
        
        culturalSite.Reviews.Add(review);
        await _apiDbContext.SaveChangesAsync();

        return Ok(new { message = "Review added successfully." });
    }
    
    [HttpGet]
    [Route("getReviews")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReviews(string culturalSiteId)
    {
        var reviews = await _apiDbContext.reviews
            .Where(r => r.CulturalSiteId == culturalSiteId && !r.IsDeleted)
            .Select(r => new ReviewResponseModel
            {
                Id = r.Id,
                CulturalSiteId = r.CulturalSiteId,
                FirstName = r.ProfileModel.FirstName,
                LastName = r.ProfileModel.LastName,
                Rating = r.Rating,
                ReviewText = r.Text,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        return Ok(reviews);
    }
    
}