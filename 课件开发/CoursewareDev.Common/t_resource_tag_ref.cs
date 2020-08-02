using System;
using System.Collections.Generic;


    public partial class t_resource_tag_ref
    {
        public long resource_id { get; set; }
        public long tag_id { get; set; }
        public sbyte? delete_flag { get; set; }
        public DateTime t_create { get; set; }
        public DateTime t_modified { get; set; }

        public virtual t_course_resource resource_ { get; set; }
        public virtual t_knowledge_tag tag_ { get; set; }
    }

