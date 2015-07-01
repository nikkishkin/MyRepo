using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManager.Core;

namespace TaskManager.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            if (UserPrincipal.CurrentPrincipal.IsManager)
            {
                return RedirectToAction(TeamsController.IndexAction, TeamsController.ControllerName);
            }

            return RedirectToAction(TasksController.IndexAction, TasksController.ControllerName);
        }

    }
}
