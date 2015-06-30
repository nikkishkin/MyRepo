using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TaskManager.Web.Models
{
    public class AddTeamModel
    {
        [Required]
        public string Name { get; set; }

        [Required, RegularExpression(@"([^,]+,{0,1})+")]
        public string Members { get; set; }

        [Required]
        public int ManagerId { get; set; }
    }
}