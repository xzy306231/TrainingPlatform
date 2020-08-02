using System;
using System.Collections.Generic;


public partial class t_item_option
{
    public long id { get; set; }
    public long questionnaire_item_id { get; set; }
    public string option_number { get; set; }
    public string option_content { get; set; }
    public sbyte? delete_flag { get; set; }
    public long? createby { get; set; }
    public DateTime? create_time { get; set; }
    public long? update_by { get; set; }
    public DateTime? update_time { get; set; }
}

