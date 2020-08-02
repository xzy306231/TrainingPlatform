using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Courseware.Core.Entities;
using Courseware.Infrastructure.Database;

namespace Courseware.Infrastructure.Repository
{
    /// <summary>
    /// 
    /// </summary>
    public class ResourceTagRepository:RepositoryBase<ResourceTagEntity>
    {
        /// <summary>
        /// 
        /// </summary>
        public ResourceTagRepository(MyContext context) : base(context)
        {
            
        }

        public async Task TryUpdateManyToMany(IEnumerable<ResourceTagEntity> currentItems,
            IEnumerable<ResourceTagEntity> newItems, Func<ResourceTagEntity, long> getKey)
        {
            DbSet.RemoveRange(currentItems.Except(newItems,getKey));
            await DbSet.AddRangeAsync(newItems.Except(currentItems, getKey));
        }
    }

    public static class DbContextExtension
    {
        public static IEnumerable<T> Except<T, TKey>(this IEnumerable<T> items, IEnumerable<T> other, Func<T, TKey> getKeyFunc)
        {
            return items
                .GroupJoin(other, getKeyFunc, getKeyFunc, (item, tempItems) => new { item, tempItems })
                .SelectMany(t => t.tempItems.DefaultIfEmpty<T>(), (t, temp) => new { t, temp })
                .Where(t => ReferenceEquals(null, t.temp) || t.temp.Equals(default(T)))
                .Select(t => t.t.item);
        }
    }
}
