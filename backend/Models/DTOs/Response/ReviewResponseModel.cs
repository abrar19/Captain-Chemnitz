namespace backend.Models.DTOs.Response;

public class ReviewResponseModel
{
    public int Id { get; set; }
    public string CulturalSiteId { get; set; }
    public string ReviewText { get; set; }  
    public int Rating { get; set; }
    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
}