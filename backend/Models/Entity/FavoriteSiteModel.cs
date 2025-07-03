using System.ComponentModel.DataAnnotations;

namespace backend.Models.Entity;

public class FavoriteSiteModel
{
    [Key]
    [Required]
    public int FavoriteSiteId { get; set; }

    public string Email { get; set; }
    public ProfileModel profileModel { get; set; }

    public string CulturalSiteId { get; set; }
    public CulturalSiteModel culturalSiteModel { get; set; }
}