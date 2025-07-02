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
    
    
  



}