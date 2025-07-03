using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace backend.Models.Entity;

public class CulturalSiteModel
{
    [Key]
    public String CulturalSiteId { get; set; }
    
    public String Name { get; set; }
    public Point Location { get; set; } 
    
    
     
    public string? Landuse { get; set; } 
    public string Museum { get; set; }  
    public string Operator { get; set; }
    public string Tourism { get; set; }
    public string Website { get; set; }
    public string Wheelchair { get; set; }
    public string Wikidata { get; set; }

    public string AddrCity { get; set; }
    public string AddrHousenumber { get; set; }
    public string AddrPostcode { get; set; }
    public string AddrStreet { get; set; }
    public string AirConditioning { get; set; }
    public string Amenity { get; set; }
    public string Bar { get; set; }
    public string Building { get; set; }
    public string BuildingLevels { get; set; }
    public string BuildingMaterial { get; set; }
    public string CheckDate { get; set; }
    public string Cuisine { get; set; }
    public string Delivery { get; set; }
    public string DietHalal { get; set; }
    public string DietKosher { get; set; }
    public string DietVegan { get; set; }
    public string DietVegetarian { get; set; }
    public string IndoorSeating { get; set; }
    public string Level { get; set; }
    public string Microbrewery { get; set; }
    public string OpeningHours { get; set; }
    public string OutdoorSeating { get; set; }
    public string PaymentCards { get; set; }
    public string PaymentCreditCards { get; set; }
    public string PaymentDebitCards { get; set; }
    public string RoofMaterial { get; set; }
    public string RoofShape { get; set; }
    public string Smoking { get; set; }
    public string Takeaway { get; set; }
    public string WebsiteMenu { get; set; }
    
    public ICollection<ReviewModel> Reviews { get; set; } = new List<ReviewModel>();
    
}