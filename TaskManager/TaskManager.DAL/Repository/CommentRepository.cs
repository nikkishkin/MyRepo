using System.Collections.Generic;
using System.Linq;
using TaskManager.Core;
using TaskManager.DAL.UnitOfWork;
using TaskManager.Entities;

namespace TaskManager.DAL.Repository
{
    public class CommentRepository
    {
        private readonly IEntityFrameworkUnitOfWork _unitOfWork;

        public CommentRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = (IEntityFrameworkUnitOfWork)unitOfWork;
        }

        public IEnumerable<Comment> GetTaskComments(int taskId)
        {
            return _unitOfWork.GetContext().Comment.Where(c => c.TaskId == taskId);
        }

        public void AddComment(Comment comment)
        {
            _unitOfWork.GetContext().Comment.Add(comment);
            _unitOfWork.GetContext().SaveChanges();
        }
    }
}
