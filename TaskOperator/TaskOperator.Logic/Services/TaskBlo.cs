﻿using System;
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

        public TaskBlo(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
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

        public void SetReadiness(int percentage, int taskId)
        {
            _taskRepository.SetReadiness(percentage, taskId);
        }

        public void SaveManagerTask(int id, string name, string content, byte state, int workerId)
        {
            _taskRepository.SaveManagerTask(id, name, content, state, workerId);
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
