namespace backend.Models.DTOs;

public class AddReviewRequestModel
{
    public string CulturalSiteId { get; set; }
    public string ReviewText { get; set; }
    public int Rating { get; set; }
    
}