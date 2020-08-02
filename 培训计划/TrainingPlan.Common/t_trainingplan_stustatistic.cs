using System;
using System.Collections.Generic;
using System.Text;


    public partial class t_trainingplan_stustatistic
    {
        public long id { get; set; }
        public long trainingplan_id { get; set; }
        public long user_id { get; set; }
        public string user_number { get; set; }
        public decimal course_comrate { get; set; }
        public decimal exam_comrate { get; set; }
        public decimal task_comrate { get; set; }
        public sbyte? delete_flag { get; set; }
        public DateTime? t_create { get; set; }
        public DateTime? t_modified { get; set; }
    }

