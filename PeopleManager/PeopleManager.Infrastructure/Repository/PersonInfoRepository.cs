using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApiUtil.Entities;
using ApiUtil.Linq;
using Microsoft.EntityFrameworkCore;
using PeopleManager.Core.Entity;
using PeopleManager.Infrastructure.Database;

namespace PeopleManager.Infrastructure.Repository
{
    public class PersonInfoRepository : RepositoryBase<PersonInfoEntity>
    {
        public PersonInfoRepository(MyContext context) : base(context)
        {
        }

        public override IQueryable<PersonInfoEntity> DataOfPage(IQueryable<PersonInfoEntity> data,
            Expression<Func<PersonInfoEntity, bool>> whereLambda, bool isNoTracking = true)
        {
            var result = base.DataOfPage(data, whereLambda, isNoTracking)
                .Include(entity => entity.WorkInfos)
                .Include(entity => entity.CertificateInfos)
                .Include(entity => entity.LicenseInfos)
                .Include(entity => entity.RewardsAndPunishments)
                .Include(entity => entity.TrainingRecords);
            return result;
        }

        public override async Task<PersonInfoEntity> GetAsync(Expression<Func<PersonInfoEntity, bool>> predicate = null, bool isNoTracking = true)
        {
            var data = isNoTracking ? _dbSet.Where(predicate).AsNoTracking() : _dbSet.Where(predicate);
            return await data
                .Include(entity => entity.WorkInfos)
                .Include(entity => entity.CertificateInfos)
                .Include(entity => entity.LicenseInfos)
                .Include(entity => entity.RewardsAndPunishments)
                .Include(entity => entity.TrainingRecords)
                .FirstOrDefaultAsync();
        }

        public async Task<List<PersonInfoEntity>> GetListIncludeFullAsync(
            Expression<Func<PersonInfoEntity, bool>> predicate = null, string ordering = "", bool isNoTracking = true)
        {
            var data = isNoTracking ? _dbSet.Where(predicate).AsNoTracking() : _dbSet.Where(predicate);
            if (!string.IsNullOrEmpty(ordering))
            {
                data = data.OrderByBatch(ordering);
            }
            return await data.Include(entity => entity.WorkInfos)
                .Include(entity => entity.CertificateInfos)
                .Include(entity => entity.LicenseInfos)
                .Include(entity => entity.RewardsAndPunishments)
                .Include(entity => entity.TrainingRecords)
                .ToListAsync();
        }
    }
}
