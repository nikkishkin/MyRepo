using System.ComponentModel.DataAnnotations;

namespace TaskManager.Web.Models.Account
{
    public class LogInModel
    {
        public string ReturnUrl { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}