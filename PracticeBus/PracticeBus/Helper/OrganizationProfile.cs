using AutoMapper;
using PracticeBus.Entity;
using PracticeBus.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PracticeBus.ViewModel.Subject;
using PracticeBus.ViewModel.SubjectScore;
using PracticeBus.ViewModel.Task;

namespace PracticeBus.Helper
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<TagDto, TKnowledgeTag>();
            CreateMap<SubjectNewDto, TSubjectBus>()
                .ForMember(opt=>opt.Description, dto=>dto.MapFrom(sb=>sb.Desc));
            CreateMap<TaskNewDto, TTaskBus>();

            CreateMap<TKnowledgeTag, TagDto>();
            CreateMap<TagDto, TKnowledgeTag>();

            CreateMap<TTaskBus, TaskQueryDto>();
            CreateMap<TSubjectBus, SubjectFullDto>();
            CreateMap<TSubjectBus, SubjectQueryDto>();

            CreateMap<TTaskBus, BaseTaskDto>();
            CreateMap<TSubjectBusScore, SubjectScoreQueryDto>()
                .ForMember(dto => dto.SubjectName, opt => opt.MapFrom(sb => sb.SubjectBus.Name))
                .ForMember(dto => dto.TagDisplay, opt => opt.MapFrom(sb => sb.SubjectBus.TagDisplay))
                .ForMember(dto => dto.Desc, opt => opt.MapFrom(sb => sb.SubjectBus.Description))
                .ForMember(dto => dto.AirplaneValue, opt => opt.MapFrom(sb => sb.SubjectBus.PlaneTypeValue))
                .ForMember(dto => dto.ClassifyValue, opt => opt.MapFrom(sb => sb.SubjectBus.ClassifyValue));
        }
    }
}
