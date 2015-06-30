using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TaskManager.Entities;

namespace TaskManager.Web.Models
{
    public class TaskModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public int Percentage { get; set; }
        public IEnumerable<UserModel> Workers { get; set; }
        public UserModel Manager { get; set; }
        public IEnumerable<CommentModel> Comments { get; set; }      
        public string Date { get; set; }
        public bool IsDone { get; set; }
    }
}