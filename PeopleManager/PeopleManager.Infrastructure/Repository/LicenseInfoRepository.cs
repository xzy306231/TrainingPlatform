using PeopleManager.Core.Entity;
using PeopleManager.Infrastructure.Database;

namespace PeopleManager.Infrastructure.Repository
{
    public class LicenseInfoRepository : RepositoryBase<LicenseInfoEntity>
    {
        public LicenseInfoRepository(MyContext context) : base(context)
        {
        }
    }
}
