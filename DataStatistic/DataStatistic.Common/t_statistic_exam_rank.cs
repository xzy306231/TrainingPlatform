using System;
using System.Collections.Generic;


    public partial class t_statistic_exam_correct_rank
    {
        public long id { get; set; }
        public string user_number { get; set; }
        public string user_name { get; set; }
        public string department { get; set; }
        public decimal? correct_rate { get; set; }
        public sbyte? delete_flag { get; set; }
        public DateTime? t_create { get; set; }
        public DateTime? t_modified { get; set; }
    }

