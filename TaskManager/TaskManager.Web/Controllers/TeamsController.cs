using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TaskManager.Core;
using TaskManager.Entities;
using TaskManager.Logic.Services;
using TaskManager.Web.Filters;
using TaskManager.Web.Models;

namespace TaskManager.Web.Controllers
{
    [Authorize, ManagerOnly]
    public class TeamsController: TaskManagerController
    {
        public const string ControllerName = "Teams";

        public const string IndexAction = "Index";
        public const string AddTeamAction = "AddTeam";

        private readonly TaskManagerBlo _manager;

        public TeamsController()
        {
            _manager = new TaskManagerBlo(UnitOfWork);
        }

        public ActionResult Index()
        {
            //if (!UserPrincipal.CurrentPrincipal.IsManager)
            //{
            //    return View("Error");
            //}

            ModelStateDictionary restoredModelState = (ModelStateDictionary)TempData["mstat"];
            if (restoredModelState != null)
            {
                ModelState.Merge(restoredModelState);
            }

            int managerId = UserPrincipal.CurrentPrincipal.UserId;
            IEnumerable<Team> teams = _manager.GetTeamsOfManager(managerId);

            TeamListModel model = new TeamListModel
            {
                Teams = teams.Select(t => new TeamModel
                {
                    Name = t.Name,
                    Id = t.Id,
                    Members = t.Members
                        .Select(w => new UserModel
                        {
                            FullName = w.First_Name + " " + w.Last_Name,
                            Id = w.Id,
                            Username = w.Username
                        })
                }),
                NewTeam = new AddTeamModel {ManagerId = managerId}
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult AddTeam(TeamListModel teamList)
        {
            if (ModelState.IsValid)
            {
                AddTeamModel teamModel = teamList.NewTeam;

                string[] invalidUsers;
                if (!_manager.WorkersExist(teamModel.Members, out invalidUsers))
                {
                    ModelState.AddModelError("Workers",
                        "Some workers were not recognized: " + String.Join(", ", invalidUsers));
                }

                if (!_manager.AreFreeWorkers(teamModel.Members, out invalidUsers))
                {
                    ModelState.AddModelError("Workers",
                        "Some workers are already in teams: " + String.Join(", ", invalidUsers));
                }

                if (invalidUsers != null)
                {
                    TempData["mstat"] = ModelState;
                    return RedirectToAction(IndexAction);
                }

                _manager.CreateTeam(teamModel.Name, teamModel.Members, teamModel.ManagerId);
                return RedirectToAction(IndexAction);
            }

            TempData["mstat"] = ModelState;
            return RedirectToAction(IndexAction);
        }
    }
}