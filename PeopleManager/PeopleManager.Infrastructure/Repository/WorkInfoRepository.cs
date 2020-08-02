using PeopleManager.Core.Entity;
using PeopleManager.Infrastructure.Database;

namespace PeopleManager.Infrastructure.Repository
{
    public class WorkInfoRepository : RepositoryBase<WorkInfoEntity>
    {
        public WorkInfoRepository(MyContext context) : base(context)
        {
        }
    }
}
