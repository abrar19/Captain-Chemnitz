namespace backend.Models.Settings;

public class ApplicationSettings
{
    public string JWT_Secret { get; set; }
    
    public string Verification_Email { get; set; }
    
    public string Verfication_Email_Pass { get; set; }
}