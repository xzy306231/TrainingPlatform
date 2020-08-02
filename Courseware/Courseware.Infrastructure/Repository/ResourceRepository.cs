using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Courseware.Core.Entities;
using Courseware.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Courseware.Infrastructure.Repository
{
    /// <summary>
    /// 
    /// </summary>
    public class ResourceRepository : RepositoryBase<ResourceEntity>
    {
        public ResourceRepository(MyContext context) : base(context)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public override IQueryable<ResourceEntity> DataOfPage(IQueryable<ResourceEntity> data, Expression<Func<ResourceEntity, bool>> whereLambda, bool isNoTracking = true)
        {
            //return base.DataOfPage(data, whereLambda, isNoTracking);
            var result = base.DataOfPage(data, whereLambda, isNoTracking).Include(entity => entity.ResourceTags).ThenInclude(entity => entity.Tag);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ResourceEntity> GetResourceAsync(Expression<Func<ResourceEntity, bool>> predicate = null, bool isNoTracking = true)
        {
            var data = isNoTracking ? DbSet.Where(predicate).AsNoTracking() : DbSet.Where(predicate);
            return await data.Include(entity=>entity.ResourceTags).ThenInclude(entity=>entity.Tag).FirstOrDefaultAsync();
        }//entity => entity.ResourceTags.Where(tagEntity => tagEntity != null && tagEntity.DeleteFlag != null && tagEntity.DeleteFlag.Equals(0))


    }
}
