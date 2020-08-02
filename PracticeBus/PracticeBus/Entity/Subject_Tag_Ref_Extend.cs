namespace PracticeBus.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public partial class TSubjectBusTagRef
    {
        public virtual TSubjectBus SubjectBus { get; set; }

        public virtual TKnowledgeTag KnowledgeTag { get; set; }
    }
}
