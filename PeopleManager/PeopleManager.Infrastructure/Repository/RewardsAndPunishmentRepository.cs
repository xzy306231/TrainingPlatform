using PeopleManager.Core.Entity;
using PeopleManager.Infrastructure.Database;

namespace PeopleManager.Infrastructure.Repository
{
    public class RewardsAndPunishmentRepository:RepositoryBase<RewardsAndPunishmentEntity>
    {
        public RewardsAndPunishmentRepository(MyContext context) : base(context)
        {
        }
    }
}
