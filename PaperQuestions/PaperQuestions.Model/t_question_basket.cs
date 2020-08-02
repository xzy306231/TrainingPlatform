using System;
using System.Collections.Generic;
using System.Text;

namespace PaperQuestions.Model
{
    public partial class t_question_basket
    {
        public long id { get; set; }
        public long? test_paper_id { get; set; }
        public long? question_id { get; set; }
        public string question_type { get; set; }
        public string complexity { get; set; }
        public decimal? question_score { get; set; }
        public  int? question_type_sort { get; set; }
        public int question_sort { get; set; }
        public string user_number { get; set; }
        public sbyte? delete_flag { get; set; }
        public DateTime? t_create { get; set; }
        public DateTime? t_modified { get; set; }
    }
}
