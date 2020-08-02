using System;
using System.Collections.Generic;


    public partial class t_paper_question_ref
    {
        public long id { get; set; }
        public long? test_paper_id { get; set; }
        public long? question_id { get; set; }
        public sbyte? delete_flag { get; set; }
        public DateTime? t_create { get; set; }
        public string create_number { get; set; }
        public DateTime? t_modified { get; set; }
    }

