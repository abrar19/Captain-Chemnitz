using System.ComponentModel.DataAnnotations;

namespace backend.Models.Entity;

public class ReviewModel
{
    [Key]
    [Required]
    public int Id { get; set; }
    
    public string Email { get; set; }
    public ProfileModel ProfileModel { get; set; }
    public string CulturalSiteId { get; set; }
    public CulturalSiteModel CulturalSiteModel { get; set; }
    
    public string Text { get; set; }
    public int Rating { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}