using System.ComponentModel.DataAnnotations;

namespace prototype_2_login_backend.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Token { get; set; }
        public string? Role { get; set; }
    }
}
