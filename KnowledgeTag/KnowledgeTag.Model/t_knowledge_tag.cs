using System;
using System.Collections.Generic;


    public partial class t_knowledge_tag
    {
        public long id { get; set; }
        public string tag { get; set; }
        public string tag_desc { get; set; }
        public long? parent_id { get; set; }
        public int? tag_sort { get; set; }
        public sbyte? delete_flag { get; set; }
        public long? create_by { get; set; }
        public DateTime? create_time { get; set; }
        public long? update_by { get; set; }
        public DateTime? update_time { get; set; }
    }

