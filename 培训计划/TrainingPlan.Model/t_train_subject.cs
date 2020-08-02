using System;
using System.Collections.Generic;


    public partial class t_train_subject
    {
    public long id { get; set; }
    public string train_number { get; set; }
    public string train_name { get; set; }
    public string train_desc { get; set; }
    public string train_kind { get; set; }
    public string plane_type_key { get; set; }
    public string create_name { get; set; }
    public string plane_type { get; set; }
    public string expect_result { get; set; }
    public sbyte? delete_flag { get; set; }
    public long? create_by { get; set; }
    public DateTime? create_time { get; set; }
    public DateTime? update_time { get; set; }
    public long? update_by { get; set; }
}

