using TaskOperator.Entities;

namespace TaskOperator.DAL.UnitOfWork
{
    public class EntityFrameworkUnitOfWork : IEntityFrameworkUnitOfWork
    {
        public TaskOperatorEntities Context { get; private set; }

        public EntityFrameworkUnitOfWork(TaskOperatorEntities context)
        {
            Context = context;
        }

        public TaskOperatorEntities GetContext()
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
