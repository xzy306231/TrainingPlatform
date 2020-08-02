using System;
using System.Collections.Generic;
using System.Text;

namespace PaperQuestions.Model
{
    public partial class t_config_paper_complexity
    {
        public long id { get; set; }
        public string paper_complexity { get; set; }
        public decimal? difficulty { get; set; }
        public decimal? general { get; set; }
        public decimal? easy { get; set; }
        public sbyte? delete_flag { get; set; }
        public DateTime? t_create { get; set; }
        public DateTime? t_modified { get; set; }
    }
}
