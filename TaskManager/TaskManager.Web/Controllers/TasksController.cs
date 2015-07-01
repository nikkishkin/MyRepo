using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TaskManager.Core;
using TaskManager.Entities;
using TaskManager.Logic.Services;
using TaskManager.Web.Models;
using TaskManager.Web.Models.Tasks;
using TaskManager.Web.Models.Users;
using Task = TaskManager.Entities.Task;

namespace TaskManager.Web.Controllers
{
    [Authorize]
    public class TasksController : TaskManagerController
    {
        public const string ControllerName = "Tasks";

        public const string IndexAction = "Index";
        public const string DetailsAction = "Details";
        public const string AddTaskAction = "AddTask";
        public const string AddTaskWorkerAction = "AddTaskWorker";
        public const string AddCommentAction = "AddComment";
        public const string SetReadinessAction = "SetReadiness";

        private const string NoTeamView = "NoTeam";

        private readonly TaskManagerBlo _manager;

        public TasksController()
        {
            _manager = new TaskManagerBlo(UnitOfWork);
        }

        //
        // GET: /Tasks/

        public ActionResult Index(int? teamId)
        {
            if (!teamId.HasValue)
            {
                User dbUser = _manager.GetUser(UserPrincipal.CurrentPrincipal.UserId);

                if (dbUser != null && dbUser.TeamId.HasValue)
                {
                    return RedirectToAction(IndexAction, new { teamId = dbUser.TeamId.Value });
                }

                return View(NoTeamView);
            }

            Team team = _manager.GetTeam(teamId.Value);

            //Verify that team exists, and current user is manager or member of team
            if (team == null ||
                (team.ManagerId != UserPrincipal.CurrentPrincipal.UserId &&
                 team.Members.All(m => m.Id != UserPrincipal.CurrentPrincipal.UserId)))
            {
                return View(ErrorView);
            }

            RestoreModelState();

            IEnumerable<TaskModel> tasks = team.Tasks
                .OrderByDescending(t => t.Date)
                .Select(t => new TaskModel
                {
                    Name = t.Name,
                    Id = t.Id,
                    Date = t.Date.ToString()
                });

            TaskListModel model = new TaskListModel
            {
                TeamName = team.Name,
                Tasks = tasks,
                NewTask = new AddTaskModel
                {
                    TeamId = teamId.Value
                },
                IsManager = UserPrincipal.CurrentPrincipal.IsManager
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult AddTask(TaskListModel taskList)
        {
            AddTaskModel taskModel = taskList.NewTask;

            if (ModelState.IsValid)
            {
                ValidateTask(taskModel);
            }

            if (ModelState.IsValid)
            {
                _manager.CreateTask(taskModel.Name, taskModel.Content, taskModel.Workers, taskModel.TeamId);
            }
            else
            {
                SaveModelState(ModelState);
            }

            return RedirectToAction(IndexAction, new {teamId = taskModel.TeamId});
        }

        private void ValidateTask(AddTaskModel taskModel)
        {
            string[] invalidUsers;
            if (!_manager.WorkersExist(taskModel.Workers, out invalidUsers))
            {
                ModelState.AddModelError("Workers",
                    "Some workers were not recognized: " + String.Join(", ", invalidUsers));
            }

            if (!_manager.AreTeamMembers(taskModel.Workers, taskModel.TeamId, out invalidUsers))
            {
                ModelState.AddModelError("Workers",
                    "Some workers are not in this team: " + String.Join(", ", invalidUsers));
            }
        }

        public ActionResult Details(int taskId)
        {
            Task task = _manager.GetTask(taskId);

            // Verify that task exists, and current user is manager or member of task's team
            if (task == null ||
                (task.Team.ManagerId != UserPrincipal.CurrentPrincipal.UserId &&
                 task.Team.Members.All(m => m.Id != UserPrincipal.CurrentPrincipal.UserId)))
            {
                return View("Error");
            }

            RestoreModelState();

            TaskDetailsModel.TaskRole role;
            if (UserPrincipal.CurrentPrincipal.IsManager)
            {
                role = TaskDetailsModel.TaskRole.Manager;
            }
            else
            {
                int userId = UserPrincipal.CurrentPrincipal.UserId;
                role = task.Workers.Any(w => w.Id == userId)
                    ? TaskDetailsModel.TaskRole.Worker
                    : TaskDetailsModel.TaskRole.Visitor;
            }

            TaskDetailsModel model = new TaskDetailsModel
            {
                NewComment = new AddCommentModel
                {
                    UserId = UserPrincipal.CurrentPrincipal.UserId,
                    TaskId = taskId
                },
                NewWorker = new AddUserModel
                {
                    TaskId = taskId
                },
                NewReadiness = new SetReadinessModel
                {
                    TaskId = taskId
                },
                Task = new TaskModel
                {
                    Content = task.Content,
                    Date = task.Date.ToString(),
                    IsDone = task.IsDone ?? false,
                    Id = taskId,
                    Percentage = task.Percentage ?? 0,
                    Workers = task.Workers
                        .Select(w => new UserModel
                        {
                            Id = w.Id,
                            FullName = w.First_Name + " " + w.Last_Name,
                            Username = w.Username
                        }),
                    Comments = task.Comments
                        .OrderByDescending(c => c.Date)
                        .Select(c => new CommentModel
                        {
                            Content = c.Content,
                            Date = c.Date.ToString(),
                            Username = c.User.Username,
                            UserFullName = String.Concat(c.User.First_Name, " ", c.User.Last_Name)
                        }),
                },
                UserRole = role
            };
            return View(model);
        }

        private void ValidateTaskWorker(AddUserModel workerModel)
        {
            if (!_manager.UserExists(workerModel.Username))
            {
                ModelState.AddModelError("Username",
                    "Worker not recognized: " + workerModel.Username.Trim());
            }

            if (!_manager.IsTeamMember(workerModel.Username, workerModel.TeamId))
            {
                ModelState.AddModelError("Username",
                    "This worker is not member of this team: " + workerModel.Username);
            }
        }

        [HttpPost]
        public ActionResult AddTaskWorker(TaskDetailsModel taskModel)
        {
            AddUserModel workerModel = taskModel.NewWorker;

            if (ModelState.IsValid)
            {
                ValidateTaskWorker(workerModel);
            }

            if (ModelState.IsValid)
            {
                _manager.AddTaskWorker(workerModel.Username, workerModel.TaskId);
            }
            else
            {
                SaveModelState(ModelState);
            }

            return RedirectToAction(DetailsAction, new {taskId = taskModel.Task.Id});
        }

        [HttpPost]
        public ActionResult AddComment(TaskDetailsModel taskModel)
        {
            AddCommentModel commentModel = taskModel.NewComment;

            if (ModelState.IsValid)
            {
                _manager.AddComment(new Comment
                {
                    Content = commentModel.Content,
                    Date = DateTime.Now,
                    TaskId = commentModel.TaskId,
                    UserId = commentModel.UserId
                });
            }
            else
            {
                SaveModelState(ModelState);
            }

            return RedirectToAction(DetailsAction, new { taskId = commentModel.TaskId });
        }

        [HttpPost]
        public ActionResult SetReadiness(TaskDetailsModel taskModel)
        {
            SetReadinessModel readinessModel = taskModel.NewReadiness;

            if (ModelState.IsValid)
            {
                _manager.SetReadiness(readinessModel.Percentage, readinessModel.TaskId);
            }
            else
            {
                SaveModelState(ModelState);
            }

            return RedirectToAction(DetailsAction, new { taskId = readinessModel.TaskId });
        }
    }
}
