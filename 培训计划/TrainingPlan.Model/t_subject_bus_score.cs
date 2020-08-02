using System;
using System.Collections.Generic;
using System.Text;


    public class t_subject_bus_score
    {
        public sbyte delete_flag { get; set; }
        public DateTime t_create { get; set; }
        public DateTime? t_modified { get; set; }
        public long id { get; set; }
        public long subject_bus_id { get; set; }
        public long original_id { get; set; }
        public long? plan_id { get; set; }
        public long task_bus_id { get; set; }
        public long task_score_id { get; set; }
        public long user_id { get; set; }
        public sbyte? status { get; set; }
        public sbyte? result { get; set; }
    }

