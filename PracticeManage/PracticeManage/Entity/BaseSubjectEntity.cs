using FreeSql.DataAnnotations;

namespace PracticeManage.Entity
{
    public class BaseSubjectEntity : BaseEntity
    {
        /// <summary>
        /// 自增主键
        /// </summary>
        [Column(IsIdentity = true,Name = "id")]
        public long Id { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        [Column(Name = "number", StringLength = 100)]
        public string Number { get; set; } = string.Empty;

        /// <summary>
        /// 训练科目名称
        /// </summary>
        [Column(Name = "name", StringLength = 200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 训练科目描述
        /// </summary>
        [Column(Name = "description", StringLength = -1)]
        public string Desc { get; set; } = string.Empty;

        /// <summary>
        /// 训练分类
        /// </summary>
        [Column(Name = "classify_key", StringLength = 20)]
        public string ClassifyKey { get; set; }

        /// <summary>
        /// 训练分类
        /// </summary>
        [Column(Name = "classify_value", StringLength = 50)]
        public string ClassifyValue { get; set; } = string.Empty;

        /// <summary>
        /// 机型key
        /// </summary>
        [Column(Name = "plane_type_key", StringLength = 20)]
        public string PlaneTypeKey { get; set; } = string.Empty;

        /// <summary>
        /// 机型value
        /// </summary>
        [Column(Name = "plane_type", StringLength = 20)]
        public string PlaneTypeValue { get; set; } = string.Empty;

        /// <summary>
        /// 期望结果
        /// </summary>
        [Column(Name = "expect_result", StringLength = -1)]
        public string ExpectResult { get; set; } = string.Empty;

        /// <summary>
        /// 创建人名
        /// </summary>
        [Column(Name = "creator_name", StringLength = 50)]
        public string CreatorName { get; set; }

        /// <summary>
        /// 创建人id
        /// </summary>
        [Column(Name = "creator_id")]
        public long CreatorId { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        [Column(Name = "version")]
        public int Version { get; set; }

        /// <summary>
        /// 知识点展示
        /// </summary>
        [Column(Name = "tag_display", StringLength = 200)]
        public string TagDisplay { get; set; } = string.Empty;
    }
}
