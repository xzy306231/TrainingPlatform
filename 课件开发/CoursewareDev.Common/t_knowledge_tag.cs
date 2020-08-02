using System;
using System.Collections.Generic;


    public partial class t_knowledge_tag
    {
        public t_knowledge_tag()
        {
            t_resource_tag_ref = new HashSet<t_resource_tag_ref>();
        }

        public long id { get; set; }
        public sbyte? delete_flag { get; set; }
        public DateTime t_create { get; set; }
        public DateTime t_modified { get; set; }
        public long original_id { get; set; }
        public string tag { get; set; }

        public virtual ICollection<t_resource_tag_ref> t_resource_tag_ref { get; set; }
    }

