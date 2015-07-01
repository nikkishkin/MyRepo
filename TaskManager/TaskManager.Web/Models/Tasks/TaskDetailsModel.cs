using TaskManager.Web.Models.Users;

namespace TaskManager.Web.Models.Tasks
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
        public SetReadinessModel NewReadiness { get; set; }
    }
}