﻿using System.Collections.Generic;
using System.Linq;
using TaskOperator.Core;
using TaskOperator.DAL.Interfaces;
using TaskOperator.DAL.UnitOfWork;
using TaskOperator.Entities;
using TaskOperator.Entities.Enums;

namespace TaskOperator.DAL.Repository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IEntityFrameworkUnitOfWork _unitOfWork;

        public TaskRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = (IEntityFrameworkUnitOfWork)unitOfWork;
        }

        public void AddTask(Task task)
        {
            _unitOfWork.GetContext().Task.Add(task);
            _unitOfWork.GetContext().SaveChanges();
        }

        public IEnumerable<Task> GetWorkerTasks(int workerId)
        {
            return _unitOfWork.GetContext().Task.Where(t => t.WorkerId == workerId).ToArray();
        }

        public IEnumerable<Task> GetAllTasks()
        {
            return _unitOfWork.GetContext().Task.ToArray();
        }

        public Task GetTask(int id)
        {
            return _unitOfWork.GetContext().Task.SingleOrDefault(t => t.Id == id);
        }

        public void SetReadiness(int percentage, int taskId)
        {
            Task task = GetTask(taskId);
            task.Percentage = percentage;
            _unitOfWork.GetContext().SaveChanges();
        }

        public void SaveManagerTask(int id, string name, string content, byte state, int workerId)
        {
            Task dbTask = _unitOfWork.GetContext().Task.Find(id);
            dbTask.Name = name;
            dbTask.Content = content;
            dbTask.State = state;

            if ((TaskState)state != TaskState.Open)
            {
                dbTask.WorkerId = workerId;
            }
            else
            {
                dbTask.WorkerId = null;
            }
            
            _unitOfWork.GetContext().SaveChanges();
        }

        public bool IsWorker(int workerId, int taskId)
        {
            Task task = GetTask(taskId);
            if ((TaskState)task.State == TaskState.Open || (TaskState)task.State == TaskState.Complete)
            {
                return false;
            }
            return task.WorkerId == workerId;
        }

        public User GetTaskWorker(Task task)
        {
            if (task.WorkerId == null)
            {
                return null;
            }
            return _unitOfWork.GetContext().User.SingleOrDefault(w => w.Id == task.WorkerId);
        }

        public User GetTaskWorker(int taskId)
        {
            Task task = GetTask(taskId);
            return GetTaskWorker(task);
        }
    }
}
