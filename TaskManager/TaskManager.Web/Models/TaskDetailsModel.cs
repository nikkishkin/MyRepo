using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManager.Web.Models
{
    public class TaskDetailsModel
    {
        public enum TaskRole
        {
            Manager, Worker, Visitor
        }

        public TaskModel Task { get; set; }
        public AddUserModel NewWorker { get; set; }
        public AddCommentModel NewComment { get; set; }
        public TaskRole UserRole { get; set; }
    }
}