using AutoMapper;
using PracticeManage.Entity;
using PracticeManage.ViewModel;
using PracticeManage.ViewModel.Subject;
using PracticeManage.ViewModel.Task;

namespace PracticeManage.Helper
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<TaskNewDto, TaskEntity>();//新建任务
            CreateMap<TaskEntity, TaskQueryDto>();//查询任务
            CreateMap<TaskEntity, TaskFullDto>();//任务详情

            CreateMap<TaskUpdateDto, TaskEntity>();

            CreateMap<SubjectCreateDto, SubjectEntity>();//创建科目
            CreateMap<SubjectEntity, SubjectFullDto>();//科目[资源]详情

            CreateMap<SubjectEditDto, SubjectEntity>()
                .AfterMap((dto, entity) => entity.Version = dto.Version + 1);//科目[资源]编辑

            CreateMap<SubjectEntity, SubjectBusNewDto>()
                .ForMember(dto => dto.OriginalId, 
                    entity => { entity.MapFrom(s => s.Id); });//科目[资源]->[业务]中间转换

            CreateMap<SubjectEntity, SubjectQueryDto>();
            

            CreateMap<SubjectBusNewDto, SubjectBusEntity>();//创建科目[业务]

            CreateMap<SubjectBusEntity, SubjectBusFullDto>();//查询科目[业务]详情

            CreateMap<TagEntity, TagDto>();
            CreateMap<TagDto, TagEntity>();
        }
    }
}
