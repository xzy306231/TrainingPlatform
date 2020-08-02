using System;
using System.Collections.Generic;


public partial class t_grade_teacher
{
    public long id { get; set; }
    public long? examination_id { get; set; }
    public string grade_teacher_num { get; set; }
    public string grade_teacher_name { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
    public int total_num { get; set; }
    public int correct_num { get; set; }
}

