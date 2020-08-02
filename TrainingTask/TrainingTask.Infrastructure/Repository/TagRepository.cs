using System;
using System.Collections.Generic;
using System.Text;
using TrainingTask.Core.Entity;
using TrainingTask.Infrastructure.Database;

namespace TrainingTask.Infrastructure.Repository
{
    public class TagRepository : RepositoryBase<TagEntity>
    {
        public TagRepository(MyContext context) : base(context)
        {
        }
    }
}
