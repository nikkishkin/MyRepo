using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaskManager.Web.Models
{
    public class AddUserModel
    {
        [Required]
        public string Username { get; set; }
        public int TeamId { get; set; }
        public int TaskId { get; set; }
    }
}