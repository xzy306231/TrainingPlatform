using FreeSql.DataAnnotations;

namespace PracticeManage.Entity
{
    [Table(Name = "t_subject_tag_ref")]
    public class SubjectTagRefEntity : BaseEntity
    {
        /// <summary>
        /// 科目id
        /// </summary>
        [Column(Name = "subject_id")]
        public long SubjectEntity_id { get; set; }
        public virtual SubjectEntity SubjectEntity { get; set; }

        /// <summary>
        /// 知识点id
        /// </summary>
        [Column(Name = "tag_id")]
        public long TagEntity_id { get; set; }
        public virtual TagEntity TagEntity { get; set; }
    }
}
