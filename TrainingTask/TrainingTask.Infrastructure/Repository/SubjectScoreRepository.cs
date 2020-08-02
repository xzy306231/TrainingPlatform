using TrainingTask.Core.Entity;
using TrainingTask.Infrastructure.Database;

namespace TrainingTask.Infrastructure.Repository
{
    public class SubjectScoreRepository : RepositoryBase<SubjectScoreEntity>
    {
        public SubjectScoreRepository(MyContext context) : base(context)
        {
        }
    }
}
