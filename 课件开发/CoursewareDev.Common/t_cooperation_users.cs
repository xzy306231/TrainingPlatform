using System;


public partial class t_cooperation_users
{
    public long id { get; set; }
    public string user_number { get; set; }
    public string user_name { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
}

