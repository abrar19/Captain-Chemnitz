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
            website = culturalSite.Website,
            Tourism = culturalSite.Tourism,
            Museum = culturalSite.Museum,
            Operator = culturalSite.Operator,
            Wheelchair = culturalSite.Wheelchair,
            Wikidata = culturalSite.Wikidata,
            Landuse = culturalSite.Landuse,
            AddrCity = culturalSite.AddrCity,
            AddrHousenumber = culturalSite.AddrHousenumber,
            AddrPostcode = culturalSite.AddrPostcode,
            AddrStreet = culturalSite.AddrStreet,
            Amenity = culturalSite.Amenity,
            OpeningHours = culturalSite.OpeningHours
            
            
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
    
  
    
    
    public string? Landuse { get; set; } 
    public string Museum { get; set; }  
    public string Operator { get; set; }
    public string Tourism { get; set; }
    public string Wheelchair { get; set; }
    public string Wikidata { get; set; }
    
    
    
    public string AddrCity { get; set; }
    public string AddrHousenumber { get; set; }
    public string AddrPostcode { get; set; }
    public string AddrStreet { get; set; }
    
    public string Amenity { get; set; }
    
    public string OpeningHours { get; set; }
    
    
}


