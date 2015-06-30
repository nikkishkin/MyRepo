using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaskManager.Web.Models
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