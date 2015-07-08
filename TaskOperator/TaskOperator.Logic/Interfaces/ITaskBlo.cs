﻿using System.Collections.Generic;
using TaskOperator.Entities;

namespace TaskOperator.Logic.Interfaces
{
    public interface ITaskBlo
    {
        void CreateTask(string name);

        IEnumerable<Task> GetWorkerTasks(int workerId);

        IEnumerable<Task> GetAllTasks();

        Task GetTask(int id);

        bool IsWorker(int workerId, int taskId);

        void SetReadiness(int percentage, int taskId);

        void SaveManagerTask(int id, string name, string content, byte state, int workerId);

        User GetTaskWorker(Task task);

        User GetTaskWorker(int taskId);
    }
}
