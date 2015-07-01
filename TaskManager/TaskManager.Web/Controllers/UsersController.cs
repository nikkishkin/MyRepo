using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TaskManager.Core;
using TaskManager.Entities;
using TaskManager.Logic.Services;
using TaskManager.Web.Filters;
using TaskManager.Web.Models;
using TaskManager.Web.Models.Users;

namespace TaskManager.Web.Controllers
{
    [Authorize, ManagerOnly]
    public class UsersController : TaskManagerController
    {
        public const string ControllerName = "Users";

        public const string IndexAction = "Index";
        public const string AddTeamMemberAction = "AddTeamMember";

        private readonly TaskManagerBlo _manager;

        public UsersController()
        {
            _manager = new TaskManagerBlo(UnitOfWork);
        }

        //
        // GET: /Users/

        public ActionResult Index(int teamId)
        {
            Team team = _manager.GetTeam(teamId);
            if (team == null || team.ManagerId != UserPrincipal.CurrentPrincipal.UserId)
            {
                return View("Error");
            }

            ModelStateDictionary restoredModelState = (ModelStateDictionary)TempData["mstat"];
            if (restoredModelState != null)
            {
                ModelState.Merge(restoredModelState);
            }

            IEnumerable<UserModel> members = _manager.GetTeam(teamId).Members
                .Select(w => new UserModel
                {
                    FullName = w.First_Name + " " + w.Last_Name,
                    Id = w.Id,
                    Username = w.Username
                });

            UserListModel model = new UserListModel
            {
                TeamName = team.Name, 
                Members = members, 
                NewUser = new AddUserModel {TeamId = teamId},
                TeamId = teamId
            };
            return View(model);
        }

        private void ValidateTeamMember(AddUserModel userModel)
        {
            if (!_manager.UserExists(userModel.Username))
            {
                ModelState.AddModelError("Username",
                    "This user was not recognized: " + userModel.Username.Trim());
            }

            if (!_manager.IsFreeWorker(userModel.Username))
            {
                ModelState.AddModelError("Username",
                    "This worker is already in some team: " + userModel.Username.Trim());
            }
        }

        [HttpPost]
        public ActionResult AddTeamMember(UserListModel userList)
        {
            AddUserModel userModel = userList.NewUser;

            if (ModelState.IsValid)
            {
                ValidateTeamMember(userModel);
            }

            if (ModelState.IsValid)
            {
                _manager.AddTeamMember(userModel.Username, userModel.TeamId);
            }
            else
            {
                SaveModelState(ModelState);
            }

            return RedirectToAction(IndexAction, new { teamId = userModel.TeamId });
        }
    }
}
