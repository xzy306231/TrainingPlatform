using System.Collections.Generic;
using FreeSql.DataAnnotations;

namespace PracticeBus.Entity
{
    public partial class TSubjectBus
    {
        [Navigate(ManyToMany = typeof(TSubjectBusTagRef))]
        public virtual List<TKnowledgeTag> Tags { get; set; }
    }
}
