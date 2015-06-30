using TaskManager.Entities;

namespace TaskManager.DAL.UnitOfWork
{
    class EntityFrameworkUnitOfWork: IEntityFrameworkUnitOfWork
    {
        public TManagerEntities Context { get; private set; }

        public EntityFrameworkUnitOfWork(TManagerEntities context)
        {
            Context = context;
        }

        public TManagerEntities GetContext()
        {
            return Context;
        }

        public void Dispose()
        {
            if (Context != null)
            {
                Context.Dispose();
            }
        }
    }
}
