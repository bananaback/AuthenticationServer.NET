using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationServer.API.Models
{
    [Table("RefreshTokens")]
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Token { get; set; }

        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
