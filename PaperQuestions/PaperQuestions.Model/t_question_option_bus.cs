﻿using System;
using System.Collections.Generic;
using System.Text;


public partial class t_question_option_bus
{
    public long id { get; set; }
    public long? question_id { get; set; }
    public string option_number { get; set; }
    public string option_content { get; set; }
    public sbyte? right_flag { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public string create_number { get; set; }
    public DateTime? t_modified { get; set; }
}

