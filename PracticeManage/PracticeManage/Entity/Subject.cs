using System.Collections.Generic;
using FreeSql.DataAnnotations;

namespace PracticeManage.Entity
{
    [Table(Name = "t_subject")]
    public class Subject : BaseSubjectEntity
    {
        [Navigate(ManyToMany = typeof(SubjectTagRefEntity))]
        public virtual List<Tag> Tags { get; set; }
    }
}
