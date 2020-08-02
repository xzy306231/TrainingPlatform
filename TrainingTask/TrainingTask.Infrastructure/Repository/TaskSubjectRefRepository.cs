using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingTask.Core.Entity;
using TrainingTask.Infrastructure.Database;

namespace TrainingTask.Infrastructure.Repository
{
    public class TaskSubjectRefRepository : RepositoryBase<TaskSubjectRefEntity>
    {
        public TaskSubjectRefRepository(MyContext context) : base(context)
        {
        }


        public async Task TryUpdateManyToMany(IEnumerable<TaskSubjectRefEntity> currentItems,
            IEnumerable<TaskSubjectRefEntity> newItems, Func<TaskSubjectRefEntity, long> getKey)
        {
            DbSet.RemoveRange(currentItems.Except(newItems, getKey));
            await DbSet.AddRangeAsync(newItems.Except(currentItems, getKey));
        }
    }
}
