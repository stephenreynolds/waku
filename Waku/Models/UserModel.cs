using System.ComponentModel.DataAnnotations;

namespace Waku.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; } = "User";

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
