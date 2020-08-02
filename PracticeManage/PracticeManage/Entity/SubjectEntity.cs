using System.Collections.Generic;
using FreeSql.DataAnnotations;

namespace PracticeManage.Entity
{
    [Table(Name = "t_subject")]
    public class SubjectEntity : BaseSubjectEntity
    {
        [Navigate(ManyToMany = typeof(SubjectTagRefEntity))]
        public virtual List<TagEntity> Tags { get; set; }
    }
}
