using System.ComponentModel.DataAnnotations;

namespace ArtGallery.Models.Account
{
    public class LogInModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public int ReturnPageNumber { get; set; }
    }
}