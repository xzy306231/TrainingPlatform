using ApiUtil.Entities;
using AutoMapper;
using PeopleManager.Api.ViewModel;
using PeopleManager.Api.ViewModel.Server;
using PeopleManager.Core.Entity;

namespace PeopleManager.Api.Common
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<PersonInfoEntity, PersonInfoDto>();
            CreateMap<PersonInfoDto, PersonInfoEntity>();

            CreateMap<PageData<PersonInfoEntity>, PageData<PersonInfoDto>>();

            CreateMap<PersonInfoEntity, SinglePersonInfoDto>().AfterMap((entity, dto) =>
            {
                if (!string.IsNullOrEmpty(dto.PhotoPath))
                    dto.PhotoPath = $"{ApiUtil.ConfigUtil.FastWebAddress}/{entity.PhotoPath}";
            });
            CreateMap<SinglePersonInfoDto, PersonInfoEntity>();

            CreateMap<PersonInfoEntity, PersonEditDto>();
            CreateMap<PersonEditDto, PersonInfoEntity>().ForMember("WorkInfos",opt=>opt.Ignore());

            CreateMap<PersonInfoEntity, TrainingPlanShowDto>().AfterMap((entity, dto) =>
            {
                if (entity.WorkInfos == null || entity.WorkInfos.Count == 0) return;
                dto.AirplaneModelValue = entity.WorkInfos[0].AirplaneModelValue;
                dto.DepartmentValue = entity.WorkInfos[0].DepartmentValue;
                dto.FlyStatusValue = entity.WorkInfos[0].FlyStatusValue;
                dto.SkillLevelValue = entity.WorkInfos[0].SkillLevelValue;
                dto.TotalDuration = entity.WorkInfos[0].TotalDuration;
            });

            CreateMap<PersonInfoEntity, TrainingPlanSelectDto>().AfterMap((entity, dto) =>
            {
                if (entity.WorkInfos == null || entity.WorkInfos.Count == 0) return;
                dto.AirplaneModelKey = entity.WorkInfos[0].AirplaneModelKey;
                dto.DepartmentKey = entity.WorkInfos[0].DepartmentKey;
                dto.FlyStatusKey = entity.WorkInfos[0].FlyStatusKey;
                dto.SkillLevelKey = entity.WorkInfos[0].SkillLevelKey;
                dto.TotalDuration = entity.WorkInfos[0].TotalDuration;
            });

            CreateMap<PageData<PersonInfoEntity>, PageData<TrainingPlanShowDto>>();
            CreateMap<PageData<PersonInfoEntity>, PageData<TrainingPlanSelectDto>>();

            CreateMap<WorkInfoEntity, WorkOfPersonDto>();
            CreateMap<WorkOfPersonDto, WorkInfoEntity>();

            CreateMap<WorkInfoEntity, WorkEditInfoDto>();
            CreateMap<WorkEditInfoDto, WorkInfoEntity>();

            CreateMap<CertificateInfoEntity, CertificateInfoDto>();
            CreateMap<CertificateInfoDto, CertificateInfoEntity>();

            CreateMap<LicenseInfoEntity, LicenseInfoDto>();
            CreateMap<LicenseInfoDto, LicenseInfoEntity>();

            CreateMap<RewardsAndPunishmentEntity, RewardsAndPunishmentDto>();
            CreateMap<RewardsAndPunishmentDto, RewardsAndPunishmentEntity>();

            CreateMap<TrainingRecordEntity, TrainingRecordDto>();
            CreateMap<TrainingRecordDto, TrainingRecordEntity>();
        }
    }
}
