namespace LoginApi.Models
{
    public class FavoritePlace
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Foreign key
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}