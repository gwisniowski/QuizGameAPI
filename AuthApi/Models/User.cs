namespace AuthApi.Models
{
    public class UserProfile
    {
        public string UserName { get; set; }
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int LastResult { get; set; }
        public int CompletedQuizes { get; set; }
    }
}
