using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courseware.Core.Entities
{
    /// <summary>
    /// 资源
    /// </summary>
    [Table("t_course_resource")]
    public class ResourceEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        /// 创建人id
        /// </summary>
        [Column("creator_id")]
        public long CreatorId { get; set; }

        /// <summary>
        /// 创建人名
        /// </summary>
        [Column("creator_name")]
        public string CreatorName { get; set; }

        /// <summary>
        /// 资源名称
        /// </summary>
        [Column("resource_name")]
        public string ResourceName { get; set; } = string.Empty;

        /// <summary>
        /// 资源描述
        /// </summary>
        [Column("resource_desc")]
        public string ResourceDesc { get; set; } = string.Empty;

        /// <summary>
        /// 资源类型id
        /// 1是视频,2是flash,3是文档,4是图片,5是SCORM课件
        /// </summary>
        [Column("resource_type")]
        public string ResourceType { get; set; } = "1";

        /// <summary>
        /// 资源时长
        /// </summary>
        [Column("resource_duration")]
        public int? ResourceDuration { get; set; } = 0;

        /// <summary>
        /// 资源密级id
        /// </summary>
        [Column("resource_level")]
        public string ResourceLevel { get; set; } = "1";

        /// <summary>
        /// 文件转换状态
        /// </summary>
        [Column("transf_type")]
        public string TransfType { get; set; }

        /// <summary>
        /// 缩略图
        /// </summary>
        [Column("thumbnail_path")]
        public string ThumbnailPath { get; set; } = string.Empty;

        /// <summary>
        /// MD5值
        /// </summary>
        [Column("md5_code")]
        public string MD5Code { get; set; } = string.Empty;

        /// <summary>
        /// 文件后缀名
        /// </summary>
        [Column("file_suffix")]
        public string FileSuffix { get; set; } = string.Empty;

        /// <summary>
        /// 资源原始路径
        /// </summary>
        [Column("original_url")]
        public string OriginalUrl { get; set; } = string.Empty;

        /// <summary>
        /// 资源转换后的路径
        /// </summary>
        [Column("transform_url")]
        public string TransformUrl { get; set; } = string.Empty;

        /// <summary>
        /// 资源文件大小，单位bite
        /// </summary>
        [Column("file_size")]
        public long FileSize { get; set; } = 0;

        /// <summary>
        /// 资源文件大小前端显示内容
        /// </summary>
        [Column("file_size_display")]
        public string FileSizeDisplay { get; set; } = string.Empty;
        
        /// <summary>
        /// 组名
        /// </summary>
        [Column("group_name")]
        public string GroupName { get; set; }

        #region ::::: SCORM相关 :::::

        /// <summary>
        /// Manifest中的title
        /// </summary>
        [Column("title_from_manifest")]
        public string TitleFromManifest { get; set; } = string.Empty;

        /// <summary>
        /// SCORM课件主页的相对路径
        /// </summary>
        [Column("path_to_index")]
        public string PathToIndex { get; set; } = string.Empty;

        /// <summary>
        /// 课件压缩资源路径
        /// </summary>
        [Column("path_to_folder")]
        public string PathToFolder { get; set; } = string.Empty;

        /// <summary>
        /// SCORM版本
        /// </summary>
        [Column("SCORM_version")]
        public string SCORMVersion { get; set; } = string.Empty;

            #endregion

        #region ::::: 审核 :::::

        /// <summary>
        /// 审核人id
        /// </summary>
        [Column("checker_id")]
        public long? CheckerId { get; set; } = 0;

        /// <summary>
        /// 审核人
        /// </summary>
        [Column("checker_name")]
        public string CheckerName { get; set; } = string.Empty;

        /// <summary>
        /// 审核时间
        /// </summary>
        [Column("check_date")]
        public DateTime? CheckDate { get; set; }

        /// <summary>
        /// 审核备注
        /// </summary>
        [Column("check_remark")]
        public string CheckRemark { get; set; } = string.Empty;

        /// <summary>
        /// 审核状态
        /// </summary>
        [Column("check_status")]
        public string CheckStatus { get; set; }

        #endregion


        /// <summary>
        /// 知识点前端显示内容
        /// </summary>
        [Column("resource_tags_display")]
        public string ResourceTagsDisplay { get; set; } = string.Empty;

        /// <summary>
        /// 标签关联表
        /// </summary>
        public virtual ICollection<ResourceTagEntity> ResourceTags { get; set; } = new List<ResourceTagEntity>();
    }
}
