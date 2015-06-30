using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TaskManager.Core;
using TaskManager.Entities;
using TaskManager.Logic.Services;
using TaskManager.Web.Models;

namespace TaskManager.Web.Controllers
{
    public class UsersController : TaskManagerController
    {
        public const string ControllerName = "Users";

        public const string IndexAction = "Index";
        public const string AddUserAction = "AddUser";

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
            if (UserPrincipal.CurrentPrincipal == null || !UserPrincipal.CurrentPrincipal.IsManager ||
                team == null || team.ManagerId != UserPrincipal.CurrentPrincipal.UserId)
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
                NewUser = new AddUserModel {TeamId = teamId}
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult AddUser(UserListModel userList)
        {
            AddUserModel userModel = userList.NewUser;

            if (ModelState.IsValid)
            {
                if (!_manager.UserExists(userModel.Username))
                {
                    ModelState.AddModelError("Username",
                        "This user was not recognized: " + userModel.Username.Trim());
                    TempData["mstat"] = ModelState;
                    return RedirectToAction(IndexAction, new { teamId = userModel.TeamId });
                }

                if (!_manager.IsFreeWorker(userModel.Username))
                {
                    ModelState.AddModelError("Username",
                        "This worker is already in some team: " + userModel.Username.Trim());
                }

                _manager.AddTeamMember(userModel.Username, userModel.TeamId);
                return RedirectToAction(IndexAction, new {teamId = userModel.TeamId});
            }

            TempData["mstat"] = ModelState;
            return RedirectToAction(IndexAction, new { teamId = userModel.TeamId });
        }
    }
}
