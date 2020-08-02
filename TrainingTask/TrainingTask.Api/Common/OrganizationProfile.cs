using ApiUtil.Entities;
using AutoMapper;
using TrainingTask.Api.ViewModel.Ref;
using TrainingTask.Api.ViewModel.Subject;
using TrainingTask.Api.ViewModel.SubjectScore;
using TrainingTask.Api.ViewModel.Tag;
using TrainingTask.Api.ViewModel.Task;
using TrainingTask.Api.ViewModel.TaskScore;
using TrainingTask.Core.Entity;

namespace TrainingTask.Api.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class OrganizationProfile : Profile
    {
        /// <summary>
        /// 
        /// </summary>
        public OrganizationProfile()
        {
            #region ::::: 获取列表 :::::

            CreateMap<TaskEntity, TaskSmipleDto>();
            CreateMap<PageData<TaskEntity>, PageData<TaskSmipleDto>>();

            #endregion

            #region ::::: 获取单个详细 :::::

            CreateMap<TaskEntity, TaskFullDto>();
            CreateMap<TaskSubjectRefEntity, TaskSubjectRefUpdateDto>();
            CreateMap<SubjectEntity, SubjectFullDto>();
            CreateMap<SubjectTagRefEntity, SubjectTagRefDto>();
            CreateMap<TagEntity, TagDto>();

            #endregion

            #region ::::: 新增/编辑 :::::

            CreateMap<TaskNewDto, TaskEntity>();
            CreateMap<TaskEntity, TaskNewDto>();

            CreateMap<TaskEntity, TaskUpdateDto>();
            CreateMap<TaskUpdateDto, TaskEntity>();

            CreateMap<TaskSubjectRefEntity, TaskSubjectRefNewDto>();
            CreateMap<TaskSubjectRefNewDto, TaskSubjectRefEntity>();

            CreateMap<SubjectEntity, SubjectNewDto>();
            CreateMap<SubjectNewDto, SubjectEntity>();

            CreateMap<SubjectTagRefDto, SubjectTagRefEntity>();

            CreateMap<TagDto, TagEntity>();

            #endregion

            #region ::::: 成绩 :::::

            CreateMap<TaskScoreNewDto, TaskScoreEntity>();
            CreateMap<SubjectScoreNewDto, SubjectScoreEntity>();

            CreateMap<TaskScoreEntity, TaskScoreRetrieveDto>().AfterMap((entity, dto) => dto.Department = FieldCheck.GetDepartment(dto.Department));
            CreateMap<PageData<TaskScoreEntity>, PageData<TaskScoreRetrieveDto>>();

            CreateMap<SubjectScoreEntity, SubjectScoreRetrieveDto>();
            CreateMap<PageData<SubjectScoreEntity>, PageData<SubjectScoreRetrieveDto>>();

            CreateMap<TaskEntity, MyTaskContentDto>();

            CreateMap<SubjectEntity, SubjectScoreCreateDto>()
                .ForMember(dto => dto.SubjectName, entity => { entity.MapFrom(s => s.Name); })
                .ForMember(dto => dto.SubjectId, entity=>entity.MapFrom(s=>s.Id));
            CreateMap<SubjectScoreCreateDto, SubjectScoreEntity >();

            CreateMap<TaskEntity, TaskScoreCreateDto>();
            CreateMap<TaskScoreCreateDto, TaskScoreEntity>();

            #endregion

            #region ::::: 统计 :::::

            CreateMap<TaskEntity, TaskStatistics>();
            CreateMap<PageData<TaskEntity>, PageData<TaskStatistics>>();

            CreateMap<SubjectEntity, SubjectStatistics>();

            #endregion

        }
    }
}
