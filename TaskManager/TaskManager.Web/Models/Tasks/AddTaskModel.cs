using System.ComponentModel.DataAnnotations;

namespace TaskManager.Web.Models.Tasks
{
    public class AddTaskModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Content { get; set; }

        [Required, RegularExpression(@"([^,]+,{0,1})+")]
        public string Workers { get; set; }

        [Required]
        public int TeamId { get; set; }
    }
}