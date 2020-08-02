using System;
using System.Collections.Generic;


    public partial class t_course_resource
    {
        public t_course_resource()
        {
            t_resource_tag_ref = new HashSet<t_resource_tag_ref>();
        }

        public long id { get; set; }
        public sbyte? delete_flag { get; set; }
        public DateTime t_create { get; set; }
        public DateTime t_modified { get; set; }
        public long creator_id { get; set; }
        public string creator_name { get; set; }
        public string resource_name { get; set; }
        public string resource_desc { get; set; }
        public string resource_type { get; set; }
        public int? resource_duration { get; set; }
        public string resource_level { get; set; }
        public string transf_type { get; set; }
        public string thumbnail_path { get; set; }
        public string md5_code { get; set; }
        public string file_suffix { get; set; }
        public string original_url { get; set; }
        public string transform_url { get; set; }
        public long file_size { get; set; }
        public string file_size_display { get; set; }
        public string group_name { get; set; }
        public string title_from_manifest { get; set; }
        public string path_to_index { get; set; }
        public string path_to_folder { get; set; }
        public string SCORM_version { get; set; }
        public long? checker_id { get; set; }
        public string checker_name { get; set; }
        public DateTime? check_date { get; set; }
        public string check_remark { get; set; }
        public string check_status { get; set; }
        public string resource_tags_display { get; set; }

        public virtual ICollection<t_resource_tag_ref> t_resource_tag_ref { get; set; }
    }

