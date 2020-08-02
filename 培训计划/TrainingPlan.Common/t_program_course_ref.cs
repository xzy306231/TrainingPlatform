using System;
using System.Collections.Generic;


    public partial class t_program_course_ref
    {
        public long id { get; set; }
        public long? programid { get; set; }
        public long? courseid { get; set; }
        public DateTime? t_create { get; set; }
        public DateTime? t_modified { get; set; }
        public sbyte? delete_flag { get; set; }
    }

