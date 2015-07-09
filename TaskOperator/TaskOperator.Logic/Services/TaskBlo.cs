using System;
using System.Collections.Generic;
using TaskOperator.Core;
using TaskOperator.DAL.Interfaces;
using TaskOperator.DAL.Repository;
using TaskOperator.Entities;
using TaskOperator.Logic.Interfaces;

namespace TaskOperator.Logic.Services
{
    public class TaskBlo: ITaskBlo
    {
        private readonly ITaskRepository _taskRepository;
        private readonly EmailService _emailService;

        public TaskBlo(ITaskRepository taskRepository, EmailService emailService)
        {
            _taskRepository = taskRepository;
            _emailService = emailService;
        }

        public void CreateTask(string name)
        {
            Task task = new Task
            {
                Name = name,
                Date = DateTime.Now
            };

            _taskRepository.AddTask(task);
        }

        public IEnumerable<Task> GetWorkerTasks(int workerId)
        {
            return _taskRepository.GetWorkerTasks(workerId);
        }

        public IEnumerable<Task> GetAllTasks()
        {
            return _taskRepository.GetAllTasks();
        }

        public Task GetTask(int id)
        {
            return _taskRepository.GetTask(id);
        }

        public bool IsWorker(int workerId, int taskId)
        {
            return _taskRepository.IsWorker(workerId, taskId);
        }

        public void SaveManagerTask(int id, string name, string content, byte state, int workerId)
        {
            Task newTask = new Task
            {
                Id = id,
                Name = name,
                Content = content,
                State = state,
                WorkerId = workerId == -1 ? (int?) null : workerId
            };

            _emailService.CheckEmailSending(GetTask(id), newTask);

            _taskRepository.SaveManagerTask(id, name, content, state, workerId);
        }

        public void SetPercentage(int id, int percentage)
        {
            _taskRepository.SetPercentage(id, percentage);
        }

        public User GetTaskWorker(Task task)
        {
            return _taskRepository.GetTaskWorker(task);
        }

        public User GetTaskWorker(int taskId)
        {
            return _taskRepository.GetTaskWorker(taskId);
        }
    }
}
