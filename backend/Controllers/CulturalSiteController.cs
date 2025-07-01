using backend.Models.Entity;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using backend.Data;
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
    
    public CulturalSiteController(APIDbContext apiDbContext)
    {
        _apiDbContext = apiDbContext;
    }
    
    [HttpGet]
    [Route("GetCulturalSites")]
    public async Task<IActionResult> GetCulturalSites()
    {
        var culturalSites = _apiDbContext.culturalSites.Select(site => new
        {
            type = "culturalSite",
            geometry = new
            {
                type = "Point",
                coordinates = new[] { site.Location.X, site.Location.Y }
            },
            properties = new
            {
                id = site.CulturalSiteId,
                name = site.Name,
                address= site.AddrCity,
                
            }
        });

        return Ok(culturalSites.ToList());
    }




}