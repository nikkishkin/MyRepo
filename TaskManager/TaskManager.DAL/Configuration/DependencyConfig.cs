using TaskManager.Core;
using TaskManager.Core.DependencyResolving;
using TaskManager.DAL.UnitOfWork;
using TaskManager.Entities;

namespace TaskManager.DAL.Configuration
{
    public static class DependencyConfig
    {
        public static void Configure()
        {
            DependencyResolver.Register<IUnitOfWork>(() => new EntityFrameworkUnitOfWork(new TManagerEntities()));
        }
    }
}
