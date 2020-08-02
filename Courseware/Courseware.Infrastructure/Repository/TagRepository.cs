using Courseware.Core.Entities;
using Courseware.Infrastructure.Database;

namespace Courseware.Infrastructure.Repository
{
    /// <summary>
    /// 
    /// </summary>
    public class TagRepository : RepositoryBase<KnowledgeTagEntity>
    {
        /// <summary>
        /// 
        /// </summary>
        public TagRepository(MyContext context) : base(context)
        {
        }
    }
}
