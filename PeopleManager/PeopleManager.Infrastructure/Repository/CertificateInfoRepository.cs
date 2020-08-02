using PeopleManager.Core.Entity;
using PeopleManager.Infrastructure.Database;

namespace PeopleManager.Infrastructure.Repository
{
    public class CertificateInfoRepository : RepositoryBase<CertificateInfoEntity>
    {
        public CertificateInfoRepository(MyContext context) : base(context)
        {
        }
    }
}
