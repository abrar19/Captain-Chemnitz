using System.Text;
using backend.Data;
using backend.Models.DTOs.Response;
using backend.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class AdminController: ControllerBase
{
    private readonly APIDbContext _apiDbContext;
    private readonly HttpClient _httpClient;
    public AdminController(APIDbContext apiDbContext,IHttpClientFactory httpClientFactory)
    {
        _apiDbContext = apiDbContext;
        _httpClient = httpClientFactory.CreateClient();

    }
    
    
    [HttpGet("SyncOverpassData")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    
    public async Task<IActionResult> GetChemnitzPointsOfInterest()
    {
        var overpassQuery = @"
  [out:json][timeout:60];
area(id:3600191645)->.searchArea;
(
  node[""tourism""=""museum""](area.searchArea);
  node[""tourism""=""artwork""](area.searchArea);
  node[""tourism""=""gallery""](area.searchArea);
  node[""art_gallery""=""yes""](area.searchArea);
  node[""amenity""=""theatre""](area.searchArea);
  node[""amenity""=""restaurant""](area.searchArea);
);
out center;
";

        var requestBody = new StringContent("data=" + Uri.EscapeDataString(overpassQuery), Encoding.UTF8, "application/x-www-form-urlencoded");

        var response = await _httpClient.PostAsync("https://overpass-api.de/api/interpreter", requestBody);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, "Failed to fetch Overpass data");
        }
        try
        {
             var result= ProcessOverpassData(content);
            return Ok("Data successfully synced from Overpass API.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error processing Overpass data: {ex.Message}");
        }
        
    }
    
    private async Task<bool> ProcessOverpassData(string content)
    {
        // Parse the JSON response and extract the relevant data
        var json = Newtonsoft.Json.Linq.JObject.Parse(content);
        var elements = json["elements"];

        List<CulturalSiteModel> culturalSites = new List<CulturalSiteModel>();
        foreach (var element in elements)
        {
            // Extract the necessary properties
            var id = element["id"].ToString();
            var type = element["type"].ToString();
            var lat = element["lat"].ToObject<double>();
            var lon = element["lon"].ToObject<double>();
            var props = element["tags"];
            var culturalSiteId = type + "/" + id;
            var point = new NetTopologySuite.Geometries.Point(lon, lat) { SRID = 4326 };

            // Create a new CulturalSiteModel instance
            var culturalSite = new CulturalSiteModel
            {
                
                  CulturalSiteId = culturalSiteId,
                Name = props["name"]?.ToString() ?? "Unknown",
                Location = point,
                
                 Landuse = props["landuse"]?.ToString() ?? "",
                Museum = props["museum"]?.ToString() ?? "",
                Operator = props["operator"]?.ToString() ?? "",
                Tourism = props["tourism"]?.ToString() ?? "",
                Website = props["website"]?.ToString() ?? "",
                Wheelchair = props["wheelchair"]?.ToString() ?? "",
                Wikidata = props["wikidata"]?.ToString() ?? "",
                AddrCity = props["addr:city"]?.ToString() ?? "",
                AddrHousenumber = props["addr:housenumber"]?.ToString() ?? "",
                AddrPostcode = props["addr:postcode"]?.ToString() ?? "",
                AddrStreet = props["addr:street"]?.ToString() ?? "",
                AirConditioning = props["air_conditioning"]?.ToString() ?? "",
                Amenity = props["amenity"]?.ToString() ?? "",
                Bar = props["bar"]?.ToString() ?? "",
                Building = props["building"]?.ToString() ?? "",
                BuildingLevels = props["building:levels"]?.ToString() ?? "",
                BuildingMaterial = props["building:material"]?.ToString() ?? "",
                CheckDate = props["check_date"]?.ToString() ?? "",
                Cuisine = props["cuisine"]?.ToString() ?? "",
                Delivery = props["delivery"]?.ToString() ?? "",
                DietHalal = props["diet:halal"]?.ToString() ?? "",
                DietKosher = props["diet:kosher"]?.ToString() ?? "",
                DietVegan = props["diet:vegan"]?.ToString() ?? "",
                DietVegetarian = props["diet:vegetarian"]?.ToString() ?? "",
                IndoorSeating = props["indoor_seating"]?.ToString() ?? "",
                Level = props["level"]?.ToString() ?? "",
                Microbrewery = props["microbrewery"]?.ToString() ?? "",
                OpeningHours = props["opening_hours"]?.ToString() ?? "",
                OutdoorSeating = props["outdoor_seating"]?.ToString() ?? "",
                PaymentCards = props["payment:cards"]?.ToString() ?? "",
                PaymentCreditCards = props["payment:credit_cards"]?.ToString() ?? "",
                PaymentDebitCards = props["payment:debit_cards"]?.ToString() ?? "",
                RoofMaterial = props["roof:material"]?.ToString() ?? "",
                RoofShape = props["roof:shape"]?.ToString() ?? "",
                Smoking = props["smoking"]?.ToString() ?? "",
                Takeaway = props["takeaway"]?.ToString() ?? "",
                WebsiteMenu = props["website:menu"]?.ToString() ?? "",
                
            };

            
            var existingSite = _apiDbContext.culturalSites
                .FirstOrDefault(s => s.CulturalSiteId == culturalSite.CulturalSiteId);
            //update 
            if (existingSite != null)
            {
                existingSite.Name = culturalSite.Name;
                existingSite.Location = culturalSite.Location;
                existingSite.Landuse = culturalSite.Landuse;
                existingSite.Museum = culturalSite.Museum;
                existingSite.Operator = culturalSite.Operator;
                existingSite.Tourism = culturalSite.Tourism;
                existingSite.Website = culturalSite.Website;
                existingSite.Wheelchair = culturalSite.Wheelchair;
                existingSite.Wikidata = culturalSite.Wikidata;
                existingSite.AddrCity = culturalSite.AddrCity;
                existingSite.AddrHousenumber = culturalSite.AddrHousenumber;
                existingSite.AddrPostcode = culturalSite.AddrPostcode;
                existingSite.AddrStreet = culturalSite.AddrStreet;
                existingSite.AirConditioning = culturalSite.AirConditioning;
                existingSite.Amenity = culturalSite.Amenity;
                existingSite.Bar = culturalSite.Bar;
                existingSite.Building = culturalSite.Building;
                existingSite.BuildingLevels = culturalSite.BuildingLevels;
                existingSite.BuildingMaterial = culturalSite.BuildingMaterial;
                existingSite.CheckDate = culturalSite.CheckDate;
                existingSite.Cuisine = culturalSite.Cuisine;
                existingSite.Delivery = culturalSite.Delivery;
                existingSite.DietHalal = culturalSite.DietHalal;
                existingSite.DietKosher = culturalSite.DietKosher;
                existingSite.DietVegan = culturalSite.DietVegan;
                existingSite.DietVegetarian = culturalSite.DietVegetarian;
                existingSite.IndoorSeating = culturalSite.IndoorSeating;
                existingSite.Level = culturalSite.Level;
                existingSite.Microbrewery = culturalSite.Microbrewery;
                existingSite.OpeningHours = culturalSite.OpeningHours;
                existingSite.OutdoorSeating = culturalSite.OutdoorSeating;
                existingSite.PaymentCards = culturalSite.PaymentCards;
                existingSite.PaymentCreditCards = culturalSite.PaymentCreditCards;
                existingSite.PaymentDebitCards = culturalSite.PaymentDebitCards;
                existingSite.RoofMaterial = culturalSite.RoofMaterial;
                existingSite.RoofShape = culturalSite.RoofShape;
                existingSite.Smoking = culturalSite.Smoking;
                existingSite.Takeaway = culturalSite.Takeaway;
                existingSite.WebsiteMenu = culturalSite.WebsiteMenu;
                
                _apiDbContext.culturalSites.Update(existingSite);
            }
            else
            {
                
                _apiDbContext.culturalSites.AddRange(culturalSite);
            }
            
        }
        
        

        var count= await  _apiDbContext.SaveChangesAsync();
        
        Console.WriteLine(count);
        return true;
    }
    


    
    //get Dashboard statistics
    [HttpGet]
    [Route("GetDashboardStatistics")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetDashboardStatistics()
    {
        var totalUsers = await _apiDbContext.applicationUsers.CountAsync()-1;//remove the admin user from the count
        var totalCulturalSites = await _apiDbContext.culturalSites.CountAsync();
        var totalInactiveUsers = await _apiDbContext.applicationUsers
            .Where(user => !user.IsActive)
            .CountAsync();
        var totalReviews = await _apiDbContext.reviews.CountAsync();

        var recentReviews = await _apiDbContext.reviews
            .Where(review => review.CreatedAt >= DateTime.UtcNow.AddDays(-30))
            .OrderByDescending(review => review.CreatedAt)
            .Take(5)
            //join user to get first and last name and culralSite for CulturalSiteName
            .Join(_apiDbContext.profiles,
                review => review.Email,
                profile => profile.Email,
                (review, profile) => new { review, profile }
            ).Join(
                _apiDbContext.culturalSites,
                reviewProfile => reviewProfile.review.CulturalSiteId,
                culturalSite => culturalSite.CulturalSiteId,
                (reviewProfile, culturalSite) => new RecentReviewResponseModel
                {
                    Id = reviewProfile.review.Id,
                    CulturalSiteId = reviewProfile.review.CulturalSiteId,
                    ReviewText = reviewProfile.review.Text,
                    Rating = reviewProfile.review.Rating,
                    FirstName = reviewProfile.profile.FirstName,
                    LastName = reviewProfile.profile.LastName,
                    CreatedAt = reviewProfile.review.CreatedAt,
                    CulturalSiteName = culturalSite.Name,


                }
            ).ToListAsync();
        
        var statistics = new AdminDashboardResponseModel
        {
            TotalUsers = totalUsers,
            TotalCulturalSites = totalCulturalSites,
            TotalReviews = totalReviews,
            TotalInactiveUsers = totalInactiveUsers,
            recentReviews = recentReviews
        };
        
        
        return Ok(statistics);
    }
    
    
    
    
    [HttpGet]
    [Route("GetAllUsers")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _apiDbContext.applicationUsers.Join(
            _apiDbContext.profiles,
            user => user.Email,
            profile => profile.Email,
            (user, profile) => new 
            {
                Email = user.Email,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                IsActive = user.IsActive,
                EmailVerified = user.EmailConfirmed,
            }
        ).ToListAsync();
            
        return Ok(users);
    }
    
    [HttpGet]
    [Route("getAllActiveUsers")]
    public async Task<IActionResult> GetAllActiveUsers()
    {
        var activeUsers = await _apiDbContext.applicationUsers
            .Where(user => user.IsActive)
            .Join(_apiDbContext.profiles,
                user => user.Email,
                profile => profile.Email,
                (user, profile) => new 
                {
                    Email = user.Email,
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    IsActive = user.IsActive,
                    EmailVerified = user.EmailConfirmed,
                }
            ).ToListAsync();
        
        return Ok(activeUsers);
    }
    
    [HttpGet]
    [Route("getAllInactiveUsers")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllInactiveUsers()
    {
        var inactiveUsers = await _apiDbContext.applicationUsers
            .Where(user => !user.IsActive)
            .Join(_apiDbContext.profiles,
                user => user.Email,
                profile => profile.Email,
                (user, profile) => new 
                {
                    Email = user.Email,
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    IsActive = user.IsActive,
                    EmailVerified = user.EmailConfirmed,
                }
            ).ToListAsync();
        
        return Ok(inactiveUsers);
    }


    
}