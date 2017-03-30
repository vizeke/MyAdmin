using System.ComponentModel.DataAnnotations;

namespace MyAdmin.Application.Models
{
    public class AuthenticationModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "RememberMe me")]
        public bool RememberMe { get; set; }
    }
}