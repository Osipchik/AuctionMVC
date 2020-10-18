using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Web.DTO
{
    public class ExternalLoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public ClaimsPrincipal Principal { get; set; }
    }
}