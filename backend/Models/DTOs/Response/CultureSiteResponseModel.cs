using backend.Models.Entity;

namespace backend.Models.DTOs.Response;

public class CultureSiteResponseModel
{
    public CultureSiteResponseModel(CulturalSiteModel culturalSite)
    {
        CulturalSiteId = culturalSite.CulturalSiteId;
        geometry = new Geometry
        {
            type = "Point",
            coordinates = new double[] { culturalSite.Location.X, culturalSite.Location.Y }
        };
        properties = new Properties
        {
            CulturalSiteId = culturalSite.CulturalSiteId,
            name = culturalSite.Name,
            website = culturalSite.Website
        };
    }
    
    
    public string CulturalSiteId { get; set; }
    public Geometry geometry { get; set; }
    public Properties properties { get; set; }
}

public class Geometry
{
    public string type { get; set; }
    public double[] coordinates { get; set; }
}

public class Properties
{
    public string CulturalSiteId { get; set; }
    public string name { get; set; }
    public string website { get; set; }
}


