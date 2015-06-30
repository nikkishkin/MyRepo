using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaskManager.Web.Models
{
    public class CommentModel
    {
        public string Content { get; set; }
        public string UserFullName { get; set; }
        public string Username { get; set; }
        public string Date { get; set; }
    }
}