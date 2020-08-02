using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUtil.Entities;
using AutoMapper;
using Courseware.Api.Common;
using Courseware.Api.ViewModel;
using Courseware.Api.ViewModel.Manage;
using Courseware.Api.ViewModel.Manage.Resource;
using Courseware.Core.Entities;
using Courseware.Infrastructure.Repository;
using LinqKit;
using Microsoft.AspNetCore.Mvc;

namespace Courseware.Api.Controllers
{
    [Route("coursesource/v1/[controller]")]
    public class TestController:Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private string _testUserName = "测试人员";
        private readonly IMapper _mapper;

        public TestController(UnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// 创建知识点种子数据
        /// </summary>
        /// <param name="count">种子数量</param>
        /// <returns></returns>
        [HttpGet("seedtags/{count}")]
        public async Task<IActionResult> CreateTagSeed(int count)
        {
            var resource = _unitOfWork.TagRepository.Count();
            if (resource < 200)
            {
                await CreateSeedTag(count);
            }

            return Ok(_unitOfWork.TagRepository.Count());
        }

        /// <summary>
        /// 清空种子数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("clearseedtags")]
        public async Task<IActionResult> DeleteTagSeed()
        {
            var resource = _unitOfWork.TagRepository.Count();
            bool result = false;
            if (resource > 0)
            {
                result = await DeleteSeedTag();
            }
            return Ok(result);
        }

        /// <summary>
        /// 创建资源种子
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet("seedResources/{count}")]
        public async Task<IActionResult> CreateResourceSeed(int count)
        {
            if (count > 0)
            {
                await CreateSeedResource(count);
            }
            return Ok(_unitOfWork.ResourceRepository.Count());
        }

        /// <summary>
        /// 清空资源种子
        /// </summary>
        /// <returns></returns>
        [HttpGet("clearseedresources")]
        public async Task<IActionResult> DeleteResourceSeed()
        {
            var resource = _unitOfWork.ResourceRepository.Count();
            bool result = false;
            if (resource > 0)
            {
                result = await DeleteSeekResource();
            }
            return Ok(result);
        }

        /// <summary>
        /// 创建资源种子和知识点种子的关系
        /// </summary>
        /// <param name="count">创建数量</param>
        /// <returns></returns>
        [HttpGet("createrelation/{count}")]
        public async Task<IActionResult> CreateRelation(int count)
        {
            var num = await CreateResourceTagRef(count);
            return Ok(num);
        }

        /// <summary>
        /// 删除关联表数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("deleterelation")]
        public async Task<IActionResult> DeleteRelation()
        {
            var deleteRef = await DeleteSeekRelation();
            return Ok(deleteRef);
        }

        [HttpGet("TestGetSomething")]
        public async Task<IActionResult> GetSomething(long userId, long tagId, string resourceType, string keyword,
            int page = 1, int perPage = 10, string sortBy = "createtime", string order = "asc")
        {

            var sortByTemp = FieldCheck.CheckFieldSortByCreateTimeSize(sortBy);
            var orderTemp = FieldCheck.CheckFieldOrder(order);
            var ordering = orderTemp + sortByTemp;

            var predicate = PredicateBuilder.New<ResourceEntity>(true).And(entity => entity.DeleteFlag == 0);//未删除

            predicate = predicate.And(entity => entity.CreatorId.Equals(userId));

            var tempCollection = await _unitOfWork.ResourceRepository.GetPageAsync(predicate, ordering, page, perPage);
            var pageData = _mapper.Map<PageData<ResourceMineDto>>(tempCollection);
            return Ok(new ResponseInfo { Result = pageData });
        }


        #region ::::: 私有函数 :::::



        #region ::::: 资源种子 :::::

        /// <summary>
        /// 创建资源种子
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private async Task<bool> CreateSeedResource(int count = 200)
        {
            var insertResources = new List<ResourceEntity>();
            for (int i = 0; i < count; i++)
            {
                long fileSize = new Random().Next(1000, 800000000);
                var tempResource = new ResourceEntity
                {
                    DeleteFlag = 0,
                    CreatorId = 9999,
                    CreatorName = _testUserName,
                    CreateTime = DateTime.Now.AddMinutes(new Random().Next(1, 10)),
                    ResourceName = $"测试资源_{i}",
                    ResourceDesc = $"测试资源{i}描述",
                    ResourceType = new Random().Next(1, 5).ToString(), //随机资源类型
                    ResourceDuration = new Random().Next(0, 20),
                    OriginalUrl = $"资源{i}的url路径",
                    ResourceLevel = FieldCheck.ResourceLevel1,
                    FileSize = fileSize,
                    FileSizeDisplay = GetFileSizeName(fileSize)
                };
                if (fileSize % 3 == 0)
                {
                    //审核通过
                    tempResource.CheckStatus = FieldCheck.PassStatus;
                    tempResource.CheckDate = DateTime.Now.AddHours(new Random().Next(1, 50));
                    tempResource.CheckerName = _testUserName;
                    tempResource.CheckRemark = $"id为{tempResource.Id}的资源已通过审核";
                    tempResource.CheckerId = Convert.ToInt64(9999);
                }
                else if (fileSize % 5 == 0)
                {
                    //审核不通过
                    tempResource.CheckStatus = FieldCheck.FailStatus;
                    tempResource.CheckDate = DateTime.Now.AddHours(new Random().Next(1, 50));
                    tempResource.CheckerName = _testUserName;
                    tempResource.CheckRemark = $"id为{tempResource.Id}的资源已被拒绝";
                    tempResource.CheckerId = Convert.ToInt64(9999);
                }
                insertResources.Add(tempResource);
            }

            return await _unitOfWork.ResourceRepository.InsertAsync(insertResources);
        }

        private string GetFileSizeName(long size)
        {
            var levelM = size / 1024d / 1024f;
            var levelG = levelM / 1024f;
            return levelG > 1 ? levelG.ToString("0.0G") : levelM > 0.1f ? levelM.ToString("0.0M") : "0.1M";
        }

        /// <summary>
        /// 删除资源种子
        /// </summary>
        /// <returns></returns>
        private async Task<bool> DeleteSeekResource()
        {
            var deleteRef = await DeleteSeekRelation();
            if (!deleteRef) return false;
            var results = _unitOfWork.ResourceRepository
                .GetListAsync(entity => entity.CreatorName.Equals(_testUserName)).Result;
            return await _unitOfWork.ResourceRepository.DeleteAsync(results);
        }

        #endregion

        #region ::::: 关联表 :::::

        /// <summary>
        /// 创建关联表种子
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private async Task<int> CreateResourceTagRef(int count)
        {
            var resources = _unitOfWork.ResourceRepository.GetListAsync(entity => !entity.ResourceTags.Any()).Result;
            var tags = _unitOfWork.TagRepository.Entities;
            for (int index = 0; index < (resources.Count >= count ? count : resources.Count); index++)
            {
                var tagContent = string.Empty;
                var tagNumb = new Random().Next(1, 10);
                var valueList = new List<long>();
                for (int i = 0; i < tagNumb; i++)
                {
                    valueList.Add(Convert.ToInt64(new Random().Next(0, count)));
                }
                valueList = valueList.Distinct().ToList();
                for (int i = 0; i < valueList.Count; i++)
                {
                    //TODO:实际情况还要考虑标签不存在的情况
                    var tempTag = tags.FirstOrDefault(tagEntity => tagEntity.OriginalId == valueList[i]);
                    tagContent += $",{tempTag.TagName}";//添加种子数据不考虑标签不存在情况
                    resources[index].ResourceTags.Add(new ResourceTagEntity
                    {
                        Tag = tempTag,
                        Resource = resources[index]
                    });
                }

                resources[index].ResourceTagsDisplay = (resources[index].ResourceTagsDisplay + tagContent).TrimStart(',');
            }
            await _unitOfWork.ResourceRepository.UpdateAsync(resources, false);
            return await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// 删除关联表种子
        /// </summary>
        /// <returns></returns>
        private async Task<bool> DeleteSeekRelation()
        {
            var resourceResults = await _unitOfWork.ResourceRepository.GetListAsync(entity => entity.ResourceTags.Any());
            if (resourceResults.Count == 0) return true;
            foreach (var result in resourceResults)
            {
                result.ResourceTags.Clear();
            }

            await _unitOfWork.ResourceRepository.UpdateAsync(resourceResults, false);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        #endregion

        #region ::::: 知识点 :::::

        /// <summary>
        /// 创建知识点种子
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private async Task<bool> CreateSeedTag(int count = 200)
        {
            var insertTags = new List<KnowledgeTagEntity>();
            for (int i = 0; i < count; i++)
            {
                insertTags.Add(new KnowledgeTagEntity
                {
                    OriginalId = i,
                    TagName = $"测试_{i}"
                });
            }

            return await _unitOfWork.TagRepository.InsertAsync(insertTags);
        }

        /// <summary>
        /// 删除知识点种子
        /// </summary>
        /// <returns></returns>
        private Task<bool> DeleteSeedTag()
        {
            return null;
            //var deleteRef = await DeleteSeekRelation();
            //if (!deleteRef) return false;
            //var results = _unitOfWork.TagRepository.GetListAsync(entity => entity.CreateUserName.Equals(_testUserName)).Result;
            //return await _unitOfWork.TagRepository.DeleteAsync(results);
        }

#endregion

        #endregion

    }
}
