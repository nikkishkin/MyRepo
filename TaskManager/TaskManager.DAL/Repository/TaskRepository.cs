using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Core;
using TaskManager.DAL.UnitOfWork;
using TaskManager.Entities;
using Task = TaskManager.Entities.Task;

namespace TaskManager.DAL.Repository
{
    public class TaskRepository
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
            return _unitOfWork.GetContext().Task.Where(t => t.Workers.Any(w => w.Id == workerId));
        }

        public Task GetTask(int id)
        {
            return _unitOfWork.GetContext().Task.SingleOrDefault(t => t.Id == id);
        } 
    }
}
