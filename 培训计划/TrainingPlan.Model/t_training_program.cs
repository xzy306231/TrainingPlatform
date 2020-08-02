using System;


public partial class t_training_program
{
    public long id { get; set; }
    public long src_id { get; set; }
    public string train_program_name { get; set; }
    public string plane_type { get; set; }
    public string train_type { get; set; }
    public string range_purpose { get; set; }
    public sbyte? purpose_visible_flag { get; set; }
    public string equipment { get; set; }
    public sbyte? equipment_visible_flag { get; set; }
    public string qualification_type { get; set; }
    public sbyte? qualification_visible_flag { get; set; }
    public string endorsement_type { get; set; }
    public string plane_type1 { get; set; }
    public int? sum_duration { get; set; }
    public int? train_time { get; set; }
    public int? up_down_times { get; set; }
    public string technical_grade { get; set; }
    public string other_remark { get; set; }
    public sbyte? enter_visible_flag { get; set; }
    public string content_question { get; set; }
    public sbyte? content_visible_flag { get; set; }
    public sbyte? course_visible_flag { get; set; }
    public sbyte? trainsubject_visible_flag { get; set; }
    public string standard_request { get; set; }
    public sbyte? request_visible_flag { get; set; }
    public sbyte? delete_flag { get; set; }
    public long? createby { get; set; }
    public DateTime? create_time { get; set; }
    public long? updateby { get; set; }
    public string update_name { get; set; }
    public DateTime? update_time { get; set; }
}

