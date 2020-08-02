using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrainingTask.Core.Entity;
using TrainingTask.Infrastructure.Database;

namespace TrainingTask.Infrastructure.Repository
{
    public class SubjectRepository : RepositoryBase<SubjectEntity>
    {
        public SubjectRepository(MyContext context) : base(context)
        {
        }

        public async Task<SubjectEntity> GetAsyncIncludeTags(Expression<Func<SubjectEntity, bool>> predicate = null, bool isNoTracking = true)
        {
            var data = isNoTracking ? DbSet.Where(predicate).AsNoTracking() : DbSet.Where(predicate);
            return await data.Include(x => x.TagRefEntities).ThenInclude(x=>x.Tag).FirstOrDefaultAsync();
        }
    }
}
