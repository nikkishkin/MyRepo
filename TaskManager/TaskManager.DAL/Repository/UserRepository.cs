using System.Linq;
using TaskManager.Core;
using TaskManager.DAL.UnitOfWork;
using TaskManager.Entities;

namespace TaskManager.DAL.Repository
{
    public class UserRepository
    {
        private readonly IEntityFrameworkUnitOfWork _unitOfWork;

        public UserRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = (IEntityFrameworkUnitOfWork)unitOfWork;
        }

        public User GetUser(int id)
        {
            return _unitOfWork.GetContext().User.Single(u => u.Id == id);
        }

        public User GetUser(string name)
        {
            return _unitOfWork.GetContext().User.Single(u => u.Username == name);
        }

        public bool UserExists(string name)
        {
            return _unitOfWork.GetContext().User.Any(u => u.Username == name);
        }

        public void AddUser(User user)
        {
            _unitOfWork.GetContext().User.Add(user);
            _unitOfWork.GetContext().SaveChanges();
        }
    }
}
