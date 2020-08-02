using System;
using System.Collections.Generic;


    public partial class t_statistic_studata
    {
        public long id { get; set; }
        public decimal learning_time { get; set; }
        public decimal training_time { get; set; }
        public int training_num { get; set; }
        public int complete_tnum { get; set; }
        public int course_num { get; set; }
        public int complete_cnum { get; set; }
        public int task_num { get; set; }
        public int complete_tasknum { get; set; }
        public decimal th_examrate { get; set; }
        public decimal? task_examrate { get; set; }
        public sbyte? delete_flag { get; set; }
        public DateTime? t_create { get; set; }
        public DateTime? t_modified { get; set; }
        public string stu_number { get; set; }
        public string dept_name { get; set; }
    }

