using System.ComponentModel.DataAnnotations;

namespace TaskManager.Web.Models.Tasks
{
    public class SetReadinessModel
    {
        [Required, Range(1, 100)]
        public int Percentage { get; set; }

        [Required, Range(0, 100)]
        public int TaskId { get; set; }
    }
}