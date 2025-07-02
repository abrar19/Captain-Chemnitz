using System.ComponentModel.DataAnnotations;

namespace backend.Models.Entity;

public class FavoriteSiteModel
{
    [Key]
    [Required]
    public int FavoriteSiteId { get; set; }

    public String Email { get; set; }
    public ProfileModel user { get; set; }

    public string CulturalSiteId { get; set; }
    public CulturalSiteModel culturalSiteModel { get; set; }
}