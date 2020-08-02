using System;
using System.Collections.Generic;
using System.Text;


    public partial class t_plancourse_node_statistic
    {
        public long id { get; set; }
        public long? plan_id { get; set; }
        public long? course_id { get; set; }
        public long? parent_id { get; set; }
        public long? node_id { get; set; }
        public string node_name { get; set; }
        public string node_type { get; set; }
        public string node_extension { get; set; }
        public int? finish_count { get; set; }
        public decimal? finish_progress { get; set; }
        public sbyte? delete_flag { get; set; }
        public DateTime? t_create { get; set; }
        public DateTime? t_modified { get; set; }
    }

