using System.ComponentModel.DataAnnotations;

namespace TaskManager.Web.Models.Users
{
    public class AddUserModel
    {
        [Required]
        public string Username { get; set; }
        public int TeamId { get; set; }
        public int TaskId { get; set; }
    }
}