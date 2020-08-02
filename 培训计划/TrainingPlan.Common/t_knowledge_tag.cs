using System;
using System.Collections.Generic;


    public partial class t_knowledge_tag
    {
        public long id { get; set; }
        public long? src_id { get; set; }
        public string tag { get; set; }
        public sbyte? delete_flag { get; set; }
        public DateTime? create_time { get; set; }
        public DateTime? update_time { get; set; }
    }

