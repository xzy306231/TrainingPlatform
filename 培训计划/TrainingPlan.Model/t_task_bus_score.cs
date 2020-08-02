using System;

    public class t_task_bus_score
    {
        public long id { get; set; }
        public long task_bus_id { get; set; }
        public long user_id { get; set; }
        public string user_name { get; set; }
        public string department { get; set; }
        public sbyte? status { get; set; }
        public sbyte? result { get; set; }
        public DateTime? start_time { get; set; }
        public DateTime? end_time { get; set; }
        public sbyte delete_flag { get; set; }
        public DateTime? t_modify { get; set; }
        public DateTime t_create { get; set; }
    }

