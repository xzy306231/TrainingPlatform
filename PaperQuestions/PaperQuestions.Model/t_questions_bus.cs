using System;
using System.Collections.Generic;
using System.Text;

public partial class t_questions_bus
{
    public long id { get; set; }
    public long? src_id { get; set; }
    public long? test_paper_id { get; set; }
    public string question_type { get; set; }
    public string complexity { get; set; }
    public string question_title { get; set; }
    public string question_desc { get; set; }
    public string question_answer { get; set; }
    public decimal? question_score { get; set; }
    public string answer_analyze { get; set; }
    public string question_confidential { get; set; }
    public int? question_type_sort { get; set; }
    public int? question_sort { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public string create_name { get; set; }
    public string create_number { get; set; }
    public DateTime? t_modified { get; set; }
}
