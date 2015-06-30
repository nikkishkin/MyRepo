using System;
using System.Collections.Generic;
using System.Linq;
using TaskManager.Core;
using TaskManager.DAL.Repository;
using TaskManager.Entities;

namespace TaskManager.Logic.Services
{
    public class TaskManagerBlo
    {
        private readonly UserRepository _userRepository;
        private readonly TaskRepository _taskRepository;
        private readonly CommentRepository _commentRepository;
        private readonly TeamRepository _teamRepository;

        public TaskManagerBlo(IUnitOfWork unitOfWork)
        {
            _userRepository = new UserRepository(unitOfWork);
            _taskRepository = new TaskRepository(unitOfWork);
            _commentRepository = new CommentRepository(unitOfWork);
            _teamRepository = new TeamRepository(unitOfWork);
        }

        public IEnumerable<Team> GetTeamsOfManager(int managerId)
        {
            return _teamRepository.GetTeamsOfManager(managerId);
        }

        public Team GetTeam(int id)
        {
            return _teamRepository.GetTeam(id);
        }

        public User GetUser(int id)
        {
            return _userRepository.GetUser(id);
        }

        public User GetUser(string username)
        {
            return _userRepository.GetUser(username.Trim());
        }

        public bool AreTeamMembers(string workersStr, int teamId, out string[] invalidUsers)
        {
            string[] workers = workersStr.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim()).ToArray();

            invalidUsers = workers.Where(w => GetUser(w).TeamForMember.Id != teamId).ToArray();

            return invalidUsers.Length == 0;
        }

        public bool IsTeamMember(int userId, int teamId)
        {
            return _teamRepository.GetTeam(teamId).Members.Any(w => w.Id == userId);
        }

        public bool IsTeamMember(string username, int teamId)
        {
            int userId = GetUser(username).Id;
            return IsTeamMember(userId, teamId);
        }

        public bool WorkersExist(string workersStr, out string[] invalidUsers)
        {
            string[] workers = workersStr.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim()).ToArray();

            invalidUsers = workers.Where(w => !UserExists(w)).ToArray();     
     
            return invalidUsers.Length == 0;
        }

        public bool UserExists(string username)
        {
            return _userRepository.UserExists(username.Trim());
        }

        /// <returns>True if there are no workers who are already in some team</returns>
        public bool AreFreeWorkers(string workersStr, out string[] invalidUsers)
        {
            string[] workers = workersStr.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim()).ToArray();

            invalidUsers = workers.Where(w => GetUser(w).TeamForMember != null).ToArray();

            return invalidUsers.Length == 0;
        }

        public bool IsFreeWorker(string username)
        {
            User worker = GetUser(username);
            return worker.TeamForMember != null;
        }

        public void AddUser(User user)
        {
            _userRepository.AddUser(user);
        }

        //public void CreateTeam(string content, string workersStr, int managerId)
        //{
        //    Task task = new Task
        //    {
        //        Content = content,
        //        ManagerId = managerId,
        //        Percentage = 0,
        //        Date = DateTime.Now
        //    };

        //    string[] workers = workersStr.Split(new [] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);
        //    foreach (string workerName in workers)
        //    {
        //        task.Workers.Add(GetUser(workerName.Trim()));
        //    }

        //    _taskRepository.AddTask(task);
        //}

        public void CreateTeam(string name, string workersStr, int managerId)
        {
            Team team = new Team
            {
                Name = name,
                ManagerId = managerId
            };

            string[] workers = workersStr.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string workerName in workers)
            {
                team.Members.Add(GetUser(workerName.Trim()));
            }

            _teamRepository.AddTeam(team);
        }

        public void CreateTask(string name, string content, string workersStr, int teamId)
        {
            Task task = new Task
            {
                Name = name,
                Content = content,
                TeamId = teamId,
                Percentage = 0,
                IsDone = false,
            };

            string[] workers = workersStr.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string workerName in workers)
            {
                task.Workers.Add(GetUser(workerName.Trim()));
            }

            _taskRepository.AddTask(task);
        }

        public IEnumerable<Task> GetWorkerTasks(int workerId)
        {
            return _taskRepository.GetWorkerTasks(workerId);
        }

        public Task GetTask(int id)
        {
            return _taskRepository.GetTask(id);
        }

        public IEnumerable<Comment> GetTaskComments(int taskId)
        {
            return _commentRepository.GetTaskComments(taskId);
        }

        public void AddComment(Comment comment)
        {
            _commentRepository.AddComment(comment);
        }

        public bool IsWorker(int userId, int taskId)
        {
            Task task = GetTask(taskId);
            return task.Workers.Any(w => w.Id == userId);
        }

        public void AddTeamMember(string username, int teamId)
        {
            Team team = GetTeam(teamId);
            team.Members.Add(GetUser(username));
        }

        public void AddTaskWorker(string username, int taskId)
        {
            Task task = GetTask(taskId);
            task.Workers.Add(GetUser(username.Trim()));
        }
    }
}
