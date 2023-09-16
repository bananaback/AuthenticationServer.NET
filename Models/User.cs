using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationServer.API.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
