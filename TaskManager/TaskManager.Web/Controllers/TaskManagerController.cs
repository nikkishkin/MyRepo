using System.Web.Mvc;
using TaskManager.Core;

namespace TaskManager.Web.Controllers
{
    public class TaskManagerController : Controller
    {
        public const string REQUEST_STORAGE_UNIT_OF_WORK = "UnitOfWork";

        public const string ErrorView = "Error";

        private const string ModelStateTempDataKey = "mstat";

        protected IUnitOfWork UnitOfWork { get; private set; }

        public TaskManagerController()
        {
            UnitOfWork = (IUnitOfWork) System.Web.HttpContext.Current.Items[REQUEST_STORAGE_UNIT_OF_WORK];
        }

        protected void SaveModelState(ModelStateDictionary modelState)
        {
            TempData[ModelStateTempDataKey] = ModelState;
        }

        protected void RestoreModelState()
        {
            ModelStateDictionary restoredModelState = (ModelStateDictionary) TempData[ModelStateTempDataKey];
            if (restoredModelState != null)
            {
                ModelState.Merge(restoredModelState);
            }
        }
    }
}
