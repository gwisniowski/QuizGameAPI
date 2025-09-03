using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public int AvatarIndex { get; set; } = 0;


        [NotMapped]
        public bool isAdmin { get; set; }
    }

    public class Admin
    {
        [Key, ForeignKey("UserProfile")]
        public int UserId { get; set; }

        public UserProfile UserProfile { get; set; }
    }



}
