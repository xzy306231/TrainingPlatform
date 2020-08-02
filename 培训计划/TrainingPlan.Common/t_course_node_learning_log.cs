using System;
using System.Collections.Generic;
using System.Text;


    public partial class t_course_node_learning_log
    {
        public long id { get; set; }
        public long? status_id { get; set; }
        public int? attempt_time { get; set; }
        public sbyte? delete_flag { get; set; }
        public DateTime? t_create { get; set; }
        public DateTime? t_modified { get; set; }
    }

