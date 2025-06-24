namespace LoginApi.Models
{
    public class FavoritePlace
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LocationId { get; set; }

        // Foreign key
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}