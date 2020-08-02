namespace Courseware.Api.ViewModel.Scorm
{
    public class LMSInfo
    {
        public LMSInfo()
        {


        }
        /// <summary>
        /// 用于标识目的的SessionID
        /// SessionID for identification purposes
        /// </summary>
        public string sessionId { get; set; }

        /// <summary>
        /// user_id
        /// </summary>
        public string userId { get; set; }

        public string coreId { get; set; }

        /// <summary>
        /// SCO的标识符(来自清单，不保证惟一)
        /// Identifier for the SCO (from the manifest, not guaranteed to be unique)
        /// </summary>
        public string scoIdentifier { get; set; }

        /// <summary>
        /// SCORM课程的标识符
        /// Identifier for the SCORM course
        /// </summary>
        public string scormCourseId { get; set; }

        /// <summary>
        /// 用于LMSSet/Get调用的DataItem
        /// DataItem for LMSSet/Get Calls
        /// </summary>
        public string dataItem { get; set; }

        /// <summary>
        /// LMSSet/Get调用的数据值
        /// Data value for LMSSet/Get calls
        /// </summary>
        public string dataValue { get; set; }

        /// <summary>
        /// 错误代码(或“0”表示没有错误)
        /// Error Code (Or "0" for no error)
        /// </summary>
        public string errorCode { get; set; }

        /// <summary>
        /// 与ErrorCode对应的错误字符串
        /// Error String corresponding to ErrorCode
        /// </summary>
        public string errorString { get; set; }

        /// <summary>
        /// 错误诊断-关于错误的附加信息
        /// Error Diagnostic - additional info about the error
        /// </summary>
        public string errorDiagnostic { get; set; }

        /// <summary>
        /// 返回给调用者的值(有时为“true”或“false”)
        /// Value to be returned to caller (sometimes this is just "true" or "false")
        /// </summary>
        public string returnValue { get; set; }

        //public override string ToString()
        //{
        //    StringBuilder s = new StringBuilder();
        //    s.Append("Core_id=");
        //    s.Append(this.coreId.ToString());
        //    s.Append("DataItem=");
        //    s.Append(this.dataItem.ToString());
        //    s.Append("DataValue=");
        //    s.Append(this.dataValue.ToString());
        //    s.Append("ErrorCode=");
        //    s.Append(this.errorCode.ToString());
        //    s.Append("ErrorDiagnostic=");
        //    s.Append(this.errorDiagnostic.ToString());
        //    s.Append("ErrorString=");
        //    s.Append(this.errorString.ToString());
        //    s.Append("ReturnValue=");
        //    s.Append(this.returnValue.ToString());
        //    s.Append("SCO_identifier=");
        //    s.Append(this.ScoIdentifier.ToString());
        //    s.Append("SCORM_course_id=");
        //    s.Append(this.ScormCourseId.ToString());
        //    s.Append("Sessionid=");
        //    s.Append(this.sessionId.ToString());
        //    s.Append("User_id=");
        //    s.Append(this.userId.ToString());
        //    return s.ToString();
        //}
    }
}
