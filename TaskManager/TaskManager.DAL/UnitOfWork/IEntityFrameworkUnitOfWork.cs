using TaskManager.Core;
using TaskManager.Entities;

namespace TaskManager.DAL.UnitOfWork
{
    interface IEntityFrameworkUnitOfWork: IUnitOfWork
    {
        TManagerEntities GetContext();
    }
}
