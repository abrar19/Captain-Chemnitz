using backend.Models.Entity;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Linq;

namespace backend.Data;

public class DBSeed
{
    public static async Task SeedData(IApplicationBuilder app,ConfigurationManager config)
{

    using var scope = app.ApplicationServices.CreateScope();


    var logger = scope.ServiceProvider.GetRequiredService<ILogger<DBSeed>>();

    try
    {
        //creating roles and superuser/adminuser
        var UserManager = scope.ServiceProvider.GetService<UserManager<ApplicationUserModel>>();
        var RoleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

        String email = config.GetSection("UserSettings")["UserEmail"];
        String password = config.GetSection("UserSettings")["UserPassword"];
        
        string[] roleNames = { "Admin", "AppUsers" };
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {

            var roleExist = await RoleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var poweruser = new ApplicationUserModel
        {
            UserName = email,
            Email = email
        };

        string userPassword = password;
        var _user = await UserManager.FindByEmailAsync(email);
        var code = await UserManager.GenerateEmailConfirmationTokenAsync(poweruser);
        var result = await UserManager.ConfirmEmailAsync(poweruser, code);

        if (_user == null)
        {
            var createProwerUser = await UserManager.CreateAsync(poweruser, userPassword);
            if (createProwerUser.Succeeded)
            {
                await UserManager.AddToRoleAsync(poweruser, "Admin");
            }
        }
        
        //prepare data
        
        var db = scope.ServiceProvider.GetRequiredService<APIDbContext>();
        if (!db.culturalSites.Any())
        {
             var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        
        

        String filePath =  Path.Combine(Directory.GetCurrentDirectory(), "Data", "Chemnitz.geojson");


        var geoJson = await File.ReadAllTextAsync(filePath);
        


        var json = JObject.Parse(geoJson);
        
        var features = json["features"];
        
        List<CulturalSiteModel> culturalSites = new List<CulturalSiteModel>();

        foreach (var feature in features)
        {
            var props = feature["properties"];
            var coords = feature["geometry"]?["coordinates"];

            var latitude = (double)coords[1];  // Latitude
            var longitude = (double)coords[0]; // Longitude
             var point = geometryFactory.CreatePoint(new Coordinate(
                longitude,  // Longitude
                latitude    // Latitude
             ));

            var site = new CulturalSiteModel()
            {
                CulturalSiteId = props["@id"].Value<string>() ?? Guid.NewGuid().ToString(),
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

           culturalSites.Add(site);
        }
        
        if (culturalSites.Count > 0)
        {
            db.culturalSites.AddRange(culturalSites);
        }
        
       await db.SaveChangesAsync();
        }


        
        

    }

      catch (Exception ex)
    {
        logger.LogCritical(ex.Message);
    }

    }
    
    
   
}