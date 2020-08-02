using System.Collections.Generic;
using System.Threading.Tasks;
using ApiUtil;
using ApiUtil.Entities;
using ApiUtil.Mq;
using AutoMapper;
using FreeSql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PracticeManage.Entity;
using PracticeManage.ViewModel;
using PracticeManage.ViewModel.Subject;

namespace PracticeManage.Controllers
{
    /// <summary>
    /// 训练科目服务
    /// </summary>
    [Route("practice/v1/subject")]
    public class SubjectManageController : Controller
    {
        private readonly IFreeSql _fSql;
        private readonly BaseRepository<SubjectEntity, long> _subjectRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SubjectManageController> _logger;
        private readonly RabbitMqClient _mqClient;
        private readonly ServiceHelper _service;

        public SubjectManageController(IFreeSql fSql, IMapper mapper,
            ILogger<SubjectManageController> logger, RabbitMqClient rabbitMq, ServiceHelper service)
        {
            _mapper = mapper;
            _logger = logger;
            _mqClient = rabbitMq;
            _service = service;
            _fSql = fSql;
            _subjectRepository = _fSql.GetRepository<SubjectEntity, long>();
        }

        /// <summary>
        /// 训练科目一览
        /// </summary>
        /// <param name="planeType">机型</param>
        /// <param name="keyword">关键字</param>
        /// <param name="tagId">知识点id</param>
        /// <param name="page">页码</param>
        /// <param name="perPage">每页条目数</param>
        /// <param name="sortBy">排序字段</param>
        /// <param name="order">排序</param>
        /// <returns></returns>
        [HttpGet("all")]
        public async Task<IActionResult> AllSubject(string planeType = "", string keyword = "", long tagId = -1, int page = 1,
            int perPage = 10, string sortBy = "createtime", string order = "desc")
        {
            var result = await _fSql.Select<SubjectEntity>()
                .WhereCascade(s => s.DeleteFlag == 0)
                .WhereIf(!string.IsNullOrEmpty(planeType), e => e.PlaneTypeValue.Equals(planeType))
                .WhereIf(!string.IsNullOrEmpty(keyword), e => e.Name.Contains(keyword))
                .IncludeMany(s => s.Tags)
                .WhereIf(tagId != -1, e => e.Tags.AsSelect().Any(t => t.OriginalId.Equals(tagId)))
                .OrderByDescending(order.Equals("desc"), e => e.CreateTime)
                .Count(out var itemCount)
                .Page(page, perPage)
                .ToListAsync(a => new SubjectQueryDto());
            return Ok(new ResponseInfo { Result = new PageData<SubjectQueryDto> { Rows = result, Totals = itemCount } });
        }

        /// <summary>
        /// 获取单个训练科目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("single")]
        public async Task<IActionResult> SingleSubject(long id)
        {
            var subjectEntity = await _fSql.Select<SubjectEntity>()
                .Where(s => s.DeleteFlag == 0 && s.Id.Equals(id))
                .IncludeMany(s => s.Tags)
                .ToOneAsync();
            return Ok(subjectEntity != null
                ? new ResponseInfo { Result = _mapper.Map<SubjectFullDto>(subjectEntity) }
                : new ResponseError("科目不存在"));
        }

        /// <summary>
        /// 新建训练科目
        /// </summary>
        /// <param name="newSubject"></param>
        /// <returns></returns>
        [HttpPost("new")]
        public async Task<IActionResult> NewSubject([FromBody]SubjectCreateDto newSubject)
        {
            if (newSubject == null || !ModelState.IsValid) return Ok(new ResponseError("参数提交错误"));
            //TODO:有说编号不能一样么？
            var existTag = newSubject.TagList != null && newSubject.TagList.Count > 0;//是否存在知识点

            #region ::::: 新增科目 :::::

            var subjectEntity = _mapper.Map<SubjectEntity>(newSubject);
            if (existTag)
            {
                var tagDisplay = string.Empty;
                foreach (var dto in newSubject.TagList)
                {
                    tagDisplay += dto.TagName + ",";
                }
                subjectEntity.TagDisplay = tagDisplay.Trim(',');
            }
            var subjectId = await _fSql.Insert(subjectEntity).ExecuteIdentityAsync();

            #endregion

            //新增知识点与关系
            var updateResult = await UpdateSubjectTagRef(subjectId, newSubject.TagList);

            //日志
            _mqClient.PushLogMessage(
                new SystemLogEntity(await _service.GetTokenInfo(), subjectId > 0 && updateResult, OptionType.Create)
                { LogDesc = $"新增科目，名称[{subjectEntity.Name}],id[{subjectId}]" });

            return Ok(new ResponseInfo());
        }

        /// <summary>
        /// 修改训练科目
        /// </summary>
        /// <param name="updateSubject"></param>
        /// <returns></returns>
        [HttpPut("edit")]
        public async Task<IActionResult> EditSubject([FromBody]SubjectEditDto updateSubject)
        {
            if (updateSubject == null || !ModelState.IsValid) return Ok(new ResponseError("参数提交错误"));

            var existTag = updateSubject.Tags != null && updateSubject.Tags.Count > 0;//是否存在知识点

            var subjectEntity = await _fSql.Select<SubjectEntity>().Where(s => s.DeleteFlag == 0 && s.Id.Equals(updateSubject.Id)).ToOneAsync();

            if (subjectEntity == null) return Ok(new ResponseError("该科目不存在，修改失败"));

            _mapper.Map(updateSubject, subjectEntity);//dto->entity

            subjectEntity.Tags.Clear();//关系表清空
            if (existTag)
            {
                var tagDisplay = string.Empty;
                foreach (var dto in updateSubject.Tags)
                {
                    tagDisplay += dto.TagName + ",";
                }
                subjectEntity.TagDisplay = tagDisplay.Trim(',');
            }
            var result = await _subjectRepository.UpdateAsync(subjectEntity);

            var updateResult = await UpdateSubjectTagRef(updateSubject.Id, updateSubject.Tags);

            _mqClient.PushLogMessage(new SystemLogEntity(await _service.GetTokenInfo(), result > 0 && updateResult)
            { LogDesc = $"更新科目,名称[{subjectEntity.Name}],id[{subjectEntity.Id}]" });
            return Ok(new ResponseInfo());
        }

        /// <summary>
        /// 删除训练科目
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveSubject(long id, string name = "")
        {
            var result = await _fSql.Update<SubjectEntity>(id).Set(a => a.DeleteFlag, 1).ExecuteAffrowsAsync();
            result = await _fSql.Update<SubjectTagRefEntity>().Where(str => str.SubjectEntity_id.Equals(id))
                .Set(str => str.DeleteFlag, 1).ExecuteAffrowsAsync();
            _mqClient.PushLogMessage(new SystemLogEntity(await _service.GetTokenInfo(), result > 0, OptionType.Delete)
            { LogDesc = $"删除科目,名称[{name}],id[{id}]" });
            return Ok(new ResponseInfo());
        }

        private async Task<bool> UpdateSubjectTagRef(long subjectId, List<TagDto> tagDtoList)
        {
            if (tagDtoList == null || tagDtoList.Count == 0) return true;

            var allTags = new List<long>(); //科目中带的所有知识点的id
            var newTags = new List<TagEntity>(); //科目中有但是表中还没有的知识点

            foreach (var dto in tagDtoList)
            {
                var temp = await _fSql.Select<TagEntity>().Where(t => t.OriginalId.Equals(dto.OriginalId)).ToOneAsync();
                if (temp != null)
                {
                    allTags.Add(temp.Id);
                    if (!temp.TagName.Equals(dto.TagName))
                        await _fSql.Update<TagEntity>(temp.Id).Set(t => t.TagName, dto.TagName).ExecuteAffrowsAsync();
                }
                else newTags.Add(_mapper.Map<TagEntity>(dto));
            }

            if (newTags.Count > 0)
            {
                foreach (var tag in newTags)
                {
                    //新增知识点id放到所有的id列表中
                    allTags.Add(await _fSql.Insert<TagEntity>().AppendData(tag).ExecuteIdentityAsync());
                }
            }

            var subjectTagRef = new List<SubjectTagRefEntity>();
            foreach (var tag in allTags)
            {
                subjectTagRef.Add(new SubjectTagRefEntity { SubjectEntity_id = subjectId, TagEntity_id = tag }); //关系表
            }

            var result = await _fSql.Insert(subjectTagRef).ExecuteAffrowsAsync();
            return result == tagDtoList.Count;
        }
    }
}
