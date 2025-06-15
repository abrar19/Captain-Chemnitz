namespace LoginApi.Models
{
    public class User
    {
        public int Id { get; set; }  // Primary key
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        
        public List<FavoritePlace> FavoritePlaces { get; set; } = new();
    }
}
