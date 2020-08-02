using System;
using System.Collections.Generic;
using System.Text;


    public class DicModel
    {
        public int Code { get; set; }
        public List<DicInfoModel> Result { get; set; }
        public string Message { get; set; }
    }

    public class DicInfoModel
    {
        public long ID { get; set; }
        public string DicCode { get; set; }
        public string CodeDsc { get; set; }
        public long ParentId { get; set; }
        //public DateTime CreateTime { get; set; }
        //public long CreateBy { get; set; }
        //public string Remark { get; set; }
    }

