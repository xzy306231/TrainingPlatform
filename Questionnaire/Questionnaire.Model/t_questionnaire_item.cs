using System;
using System.Collections.Generic;


public partial class t_questionnaire_item
{
    public long id { get; set; }
    public long questionnnaire_id { get; set; }
    public string item_title { get; set; }
    public string item_type { get; set; }
    public string item_type_desc { get; set; }
    public int? item_sort { get; set; }
    public sbyte? must_answer_flag { get; set; }
    public int? min_answer_num { get; set; }
    public int? max_answer_num { get; set; }
    public sbyte? delete_flag { get; set; }
    public long? create_by { get; set; }
    public DateTime? create_time { get; set; }
    public long? update_by { get; set; }
    public DateTime? update_time { get; set; }
}

