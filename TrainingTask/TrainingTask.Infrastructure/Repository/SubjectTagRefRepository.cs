using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingTask.Core.Entity;
using TrainingTask.Infrastructure.Database;

namespace TrainingTask.Infrastructure.Repository
{
    public class SubjectTagRefRepository :RepositoryBase<SubjectTagRefEntity>
    {
        public SubjectTagRefRepository(MyContext context) : base(context)
        {
        }

        public async Task TryUpdateManyToMany(IEnumerable<SubjectTagRefEntity> currentItems,
            IEnumerable<SubjectTagRefEntity> newItems, Func<SubjectTagRefEntity, long> getKey)
        {
            DbSet.RemoveRange(currentItems.Except(newItems, getKey));
            await DbSet.AddRangeAsync(newItems.Except(currentItems, getKey));
        }
    }
}
