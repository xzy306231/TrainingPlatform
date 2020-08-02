using ApiUtil.Entities;
using AutoMapper;
using Courseware.Api.ViewModel.FileUpDown;
using Courseware.Api.ViewModel.Manage;
using Courseware.Api.ViewModel.Manage.Resource;
using Courseware.Core.Entities;

namespace Courseware.Api.Common
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            //知识点Dto
            CreateMap<KnowledgeTagEntity, KnowledgeTagDto>();
            CreateMap<KnowledgeTagDto, KnowledgeTagEntity>();

            //Resource和全部课件互转
            CreateMap<ResourceEntity, ResourceAllDto>().AfterMap((entity, dto) =>
            {
                dto.ResourceUrl = $"{ApiUtil.ConfigUtil.FastWebAddress}/{entity.GroupName}/" + entity.TransformUrl;
            });
            CreateMap<ResourceAllDto, ResourceEntity>();

            //Resource和我的课件互转
            CreateMap<ResourceEntity, ResourceMineDto>().AfterMap((entity, dto) =>
                dto.ResourceUrl = $"{ApiUtil.ConfigUtil.FastWebAddress}/{entity.GroupName}/" + entity.TransformUrl);
            CreateMap<ResourceMineDto, ResourceEntity>();

            //Resource转已审核课件
            CreateMap<ResourceEntity, ResourceCheckedDto>().AfterMap((entity, dto) =>
                dto.ResourceUrl = $"{ApiUtil.ConfigUtil.FastWebAddress}/{entity.GroupName}/" + entity.TransformUrl);
            //Resource转未审核课件
            CreateMap<ResourceEntity, ResourceUnCheckedDto>().AfterMap((entity, dto) =>
                dto.ResourceUrl = $"{ApiUtil.ConfigUtil.FastWebAddress}/{entity.GroupName}/" + entity.TransformUrl);

            //资源标签关联表互转
            CreateMap<ResourceTagEntity, ResourceTagDto>();
            CreateMap<ResourceTagDto, ResourceTagEntity>();

            //上传信息与课件信息转换
            CreateMap<UploadFileInfo, ResourceEntity>().AfterMap((info, entity) =>
            {
                entity.TransfType = FieldCheck.CheckSuffixStr(entity.FileSuffix);
                entity.TransformUrl = FieldCheck.CheckSuffix(entity.FileSuffix) ? entity.OriginalUrl : string.Empty;
                entity.CheckStatus = FieldCheck.CheckingStatus;
                entity.ResourceName = FieldCheck.SplitResourceName(entity.ResourceName);
                entity.ResourceTagsDisplay = "暂无知识点";
            });

            //
            CreateMap<PageData<ResourceEntity>, PageData<ResourceAllDto>>();

            CreateMap<PageData<ResourceEntity>, PageData<ResourceMineDto>>();

            CreateMap<PageData<ResourceEntity>, PageData<ResourceCheckedDto>>();

            CreateMap<PageData<ResourceEntity>, PageData<ResourceUnCheckedDto>>();

        }
    }
}
