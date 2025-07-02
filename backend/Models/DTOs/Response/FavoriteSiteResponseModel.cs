using backend.Models.Entity;

namespace backend.Models.DTOs.Response;

public class FavoriteSiteResponseModel
{

    public FavoriteSiteResponseModel(int FavoriteSiteId,CultureSiteResponseModel culturalSite)
    {
        this.FavoriteSiteId = FavoriteSiteId;
        CulturalSite = culturalSite ?? throw new ArgumentNullException(nameof(culturalSite), "Cultural site cannot be null");
    }
  
    
    public int FavoriteSiteId { get; set; }
    public CultureSiteResponseModel CulturalSite { get; set; }
}