using System.Collections.Generic;
using FreeSql.DataAnnotations;

namespace PracticeBus.Entity
{
    public partial class TKnowledgeTag
    {
        [Navigate(ManyToMany = typeof(TSubjectBusTagRef))]
        public virtual List<TSubjectBus> Subjects { get; set; }
    }
}
