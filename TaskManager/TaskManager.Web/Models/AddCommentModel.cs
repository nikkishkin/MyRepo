using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaskManager.Web.Models
{
    public class AddCommentModel
    {
        [Required]
        public string Content { get; set; }
        public int TaskId { get; set; }
        public int UserId { get; set; }
    }
}