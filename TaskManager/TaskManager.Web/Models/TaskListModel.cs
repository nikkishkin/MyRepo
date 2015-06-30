using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManager.Web.Models
{
    public class TaskListModel
    {
        public string TeamName { get; set; }
        public IEnumerable<TaskModel> Tasks { get; set; }
        public AddTaskModel NewTask { get; set; }
        public bool IsManager { get; set; }
    }
}