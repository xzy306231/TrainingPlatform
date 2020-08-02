using System;
using System.Collections.Generic;


    public partial class t_program_subject_ref
    {
        public long id { get; set; }
        public long? programid { get; set; }
        public long? subjectid { get; set; }
        public DateTime? t_create { get; set; }
        public DateTime? t_modified { get; set; }
        public sbyte? delete_flag { get; set; }
    }

