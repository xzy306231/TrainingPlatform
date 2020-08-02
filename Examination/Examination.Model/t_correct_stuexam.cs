using System;
using System.Collections.Generic;


    public partial class t_correct_stuexam
    {
        public int id { get; set; }
        public int? gradetea_id { get; set; }
        public string user_number { get; set; }
        public string uesr_name { get; set; }
        public int correct_statu { get; set; }
        public DateTime? correct_date { get; set; }
        public int examination_id { get; set; }
        public sbyte? delete_flag { get; set; }
        public DateTime? t_create { get; set; }
        public DateTime? t_modified { get; set; }
    }

