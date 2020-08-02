using System;
using System.Collections.Generic;
using System.Text;

public partial class t_question_tag_bus_ref
{
    public long id { get; set; }
    public long? question_id { get; set; }
    public long? knowledge_tag_id { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
}
