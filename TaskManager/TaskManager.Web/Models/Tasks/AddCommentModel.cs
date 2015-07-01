using System.ComponentModel.DataAnnotations;

namespace TaskManager.Web.Models.Tasks
{
    public class AddCommentModel
    {
        [Required]
        public string Content { get; set; }
        public int TaskId { get; set; }
        public int UserId { get; set; }
    }
}