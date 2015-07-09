﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using TaskOperator.Core;
using TaskOperator.Entities;
using TaskOperator.Entities.Enums;
using TaskOperator.Logic.Interfaces;
using TaskOperator.Web.Models;
using TaskOperator.Web.Models.Tasks;

namespace TaskOperator.Web.Controllers
{
    public class TasksController : TaskOperatorController
    {
        public const string ControllerName = "Tasks";

        public const string ManagerTasksAction = "ManagerTasks";
        public const string WorkerTasksAction = "WorkerTasks";

        public const string AddTaskAction = "AddTask";

        public const string SaveManagerTaskAction = "SaveManagerTask";
        public const string GetManagerTaskAction = "GetManagerTask";

        public const string SaveWorkerTaskAction = "SaveWorkerTask";
        public const string GetWorkerTaskAction = "GetWorkerTask";

        private const string NoUserString = "No user";
        private const int NoUserId = -1;

        private readonly ITaskBlo _taskBlo;
        private readonly IUserBlo _userBlo;

        public TasksController(IUserBlo userBlo, ITaskBlo taskBlo)
        {
            _taskBlo = taskBlo;
            _userBlo = userBlo;
        }

        [HttpPost]
        public PartialViewResult GetTasks()
        {
            IEnumerable<TaskModel> tasks;
            if (UserPrincipal.CurrentPrincipal.IsManager)
            {
                // User is manager
                tasks = _taskBlo.GetAllTasks().Select(GetTaskModel);
                return PartialView("_ManagerTasks", tasks);
            }

            tasks = _taskBlo.GetWorkerTasks(UserPrincipal.CurrentPrincipal.UserId).Select(GetTaskModel);
            return PartialView("_WorkerTasks", tasks);
        }

        private UserModel GetUserModel(Task task)
        {
            if (task.State == 0)
            {
                // Task is open and has no workers
                return null;
            }

            User worker = _taskBlo.GetTaskWorker(task);
            return new UserModel
            {
                FullName = worker.First_Name + " " + worker.Last_Name,
                Id = worker.Id,
                Username = worker.Username
            };
        }

        [HttpPost]
        public PartialViewResult AddTask(AddTaskModel taskModel)
        {
            if (ModelState.IsValid)
            {
                _taskBlo.CreateTask(taskModel.Name);
            }

            return PartialView("_AddTask");
        }

        /// <summary>
        /// Fills task model properties except collections - FreeWorkers and StateOptions
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private TaskModel GetTaskModel(Task task)
        {
            return new TaskModel
            {
                Name = task.Name,
                Id = task.Id,
                State = task.State,
                Content = task.Content,
                Percentage = task.Percentage,
                Date = task.Date.ToString(CultureInfo.CurrentCulture),
                Worker = GetUserModel(task),
                IsAssigned = (TaskState)task.State != TaskState.Open
            };
        }

        //[HttpPost]
        public PartialViewResult GetManagerTask(int taskId)
        {
            TaskModel model = GetTaskModel(_taskBlo.GetTask(taskId));

            FillCollections(model);

            return PartialView("_ManagerTask", model);
        }

        public PartialViewResult GetWorkerTask(int taskId)
        {
            TaskModel model = GetTaskModel(_taskBlo.GetTask(taskId));

            return PartialView("_WorkerTask", model);
        }

        private void FillCollections(TaskModel model)
        {
            model.WorkerOptions = GetWorkerOptions(model);
            model.StateOptions = GetStateOptions();
        }

        private IEnumerable<SelectListItem> GetWorkerOptions(TaskModel model)
        {
            List<SelectListItem> workerOtions = new List<SelectListItem>();
            workerOtions.AddRange(_userBlo.GetFreeWorkers().Select(w => new SelectListItem {Text = w.Username, Value = w.Id.ToString()}));

            SelectListItem noUserItem = new SelectListItem { Text = NoUserString, Value = NoUserId.ToString() };

            if (model.Worker == null || model.Worker.Id == NoUserId)
            {
                // Task is open and has no workers
                noUserItem.Selected = true;
            }
            else if ((TaskState)model.State != TaskState.Complete || !_userBlo.IsFreeWorker(model.Worker.Id))
            {
                User dbUser = _userBlo.GetUser(model.Worker.Id);
                workerOtions.Add(new SelectListItem {Text = dbUser.Username, Value = dbUser.Id.ToString(), Selected = true});
            }

            workerOtions.Add(noUserItem);

            return workerOtions;
        }

        private static IEnumerable<SelectListItem> GetStateOptions()
        {
            return Enum.GetValues(typeof (TaskState)).Cast<TaskState>().Select(x => new SelectListItem
            {
                Value = ((byte) x).ToString(),
                Text = x.ToString()
            });
        }

        private void ValidateTaskModel(TaskModel taskModel)
        {
            if ((TaskState)taskModel.State == TaskState.Open && taskModel.Worker.Id != NoUserId)
            {
                ModelState.AddModelError("StateString", "State cannot be 'open' because task is assigned to worker");
                return;
            }
            if (taskModel.Worker.Id == NoUserId && (TaskState)taskModel.State != TaskState.Open)
            {
                ModelState.AddModelError("StateString", "State must be 'open' because task has no workers");
                return;
            }
            if (taskModel.Worker.Id != NoUserId && !_taskBlo.IsWorker(taskModel.Worker.Id, taskModel.Id) &&
                !_userBlo.IsFreeWorker(taskModel.Worker.Id))
            {
                ModelState.AddModelError("Worker", "Selected worker is busy");
            }
        }

        [HttpPost]
        public PartialViewResult SaveManagerTask(TaskModel taskModel)
        {
            ValidateTaskModel(taskModel);

            if (ModelState.IsValid)
            {
                _taskBlo.SaveManagerTask(taskModel.Id, taskModel.Name, taskModel.Content,
                         taskModel.State, taskModel.Worker.Id);
            }

            FillCollections(taskModel);

            return PartialView("_ManagerTask", taskModel);
        }

        [HttpPost]
        public PartialViewResult SaveWorkerTask(TaskModel taskModel)
        {
            if (ModelState.IsValid)
            {
                _taskBlo.SetPercentage(taskModel.Id, taskModel.Percentage);
            }

            taskModel = GetTaskModel(_taskBlo.GetTask(taskModel.Id));

            return PartialView("_WorkerTask", taskModel);
        }
    }
}
