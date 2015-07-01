using System.Collections.Generic;
using TaskManager.Web.Models.Users;

namespace TaskManager.Web.Models.Tasks
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