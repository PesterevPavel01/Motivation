using System.ComponentModel.DataAnnotations;

namespace Motivation.IdentityServer.Web.Pages.Connect
{
    public class LoginViewModel
    {

        [Required]
        [Display(Name = "Логин")]
        public string UserName { get; set; } = null!;

        [Required]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = null!;

        [Required]
        public string? ReturnUrl { get; set; } = null!;
    }
}
