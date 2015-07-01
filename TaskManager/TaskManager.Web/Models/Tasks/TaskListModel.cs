using System.Collections.Generic;

namespace TaskManager.Web.Models.Tasks
{
    public class TaskListModel
    {
        public string TeamName { get; set; }
        public IEnumerable<TaskModel> Tasks { get; set; }
        public AddTaskModel NewTask { get; set; }
        public bool IsManager { get; set; }
    }
}