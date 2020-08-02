using PeopleManager.Core.Entity;
using PeopleManager.Infrastructure.Database;

namespace PeopleManager.Infrastructure.Repository
{
    public class TrainingRecordRepository : RepositoryBase<TrainingRecordEntity>
    {
        public TrainingRecordRepository(MyContext context) : base(context)
        {
        }
    }
}
