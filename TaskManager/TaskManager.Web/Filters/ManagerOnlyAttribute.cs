using System.Web.Mvc;
using TaskManager.Core;
using TaskManager.Web.Controllers;

namespace TaskManager.Web.Filters
{
    public class ManagerOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            UserPrincipal principal = UserPrincipal.CurrentPrincipal;
            if (principal == null || !principal.IsManager)
            {
                filterContext.Result = new ViewResult {ViewName = TaskManagerController.ErrorView};
            }

            base.OnActionExecuting(filterContext);
        }
    }
}