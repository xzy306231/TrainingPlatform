using System;
using System.Collections.Generic;
using System.Text;


    public class t_subject_knowledge_ref
    {
    public long id { get; set; }
    public long? subject_id { get; set; }
    public long? knowledge_tag_id { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
}

