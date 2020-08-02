using System;
using System.Collections.Generic;


public partial class t_interaction_log
{
    public long id { get; set; }
    public long? questionnaire_id { get; set; }
    public long? questionnaire_item_id { get; set; }
    public long item_option_id { get; set; }
    public long? participate_id { get; set; }
    public string participate_name { get; set; }
    public string interaction_result { get; set; }
    public DateTime? interactive_time { get; set; }
}

