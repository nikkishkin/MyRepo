using System.Collections.Generic;
using System.Linq;
using TaskManager.Core;
using TaskManager.DAL.UnitOfWork;
using TaskManager.Entities;

namespace TaskManager.DAL.Repository
{
    public class TeamRepository
    {
        private readonly IEntityFrameworkUnitOfWork _unitOfWork;

        public TeamRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = (IEntityFrameworkUnitOfWork)unitOfWork;
        }

        public IEnumerable<Team> GetTeamsOfManager(int managerId)
        {
            return _unitOfWork.GetContext().Team.Where(t => t.ManagerId == managerId);
        }

        public void AddTeam(Team team)
        {
            _unitOfWork.GetContext().Team.Add(team);
        }

        public Team GetTeam(int id)
        {
            return _unitOfWork.GetContext().Team.SingleOrDefault(t => t.Id == id);
        }
    }
}
