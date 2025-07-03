namespace backend.Models.DTOs.Response;

public class AdminDashboardResponseModel
{
    public int TotalUsers { get; set; }
    public int TotalCulturalSites { get; set; }
    public int TotalReviews { get; set; }
    public int TotalInactiveUsers { get; set; }

    public List<RecentReviewResponseModel> recentReviews   { get; set; }

}

public class RecentReviewResponseModel
{
    public int Id { get; set; }
    public string CulturalSiteId { get; set; }
    public string ReviewText { get; set; }  
    public int Rating { get; set; }
    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public string CulturalSiteName { get; set; }
}