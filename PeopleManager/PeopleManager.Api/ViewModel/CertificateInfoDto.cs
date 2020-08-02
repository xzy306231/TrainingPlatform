using System;

namespace PeopleManager.Api.ViewModel
{
    public class CertificateInfoDto
    {
        /// <summary>
        /// 执照名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 执照编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 执照类型Key
        /// </summary>
        public string TypeKey { get; set; }

        /// <summary>
        /// 机型key
        /// </summary>
        public string AirplaneModelKey { get; set; }

        /// <summary>
        /// 机型value
        /// </summary>
        public string AirplaneModelValue { get; set; }

        /// <summary>
        /// 执照类型Value
        /// </summary>
        public string TypeValue { get; set; }

        /// <summary>
        /// 是否有效Key
        /// </summary>
        public string ValidKey { get; set; }

        /// <summary>
        /// 是否有效Value
        /// </summary>
        public string ValidValue { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? GetDate { get; set; }

        /// <summary>
        /// 最后签注日期
        /// </summary>
        public DateTime? LastEndorseDate { get; set; }

        /// <summary>
        /// 过期日期
        /// </summary>
        public DateTime? ExpirationDate { get; set; }
    }
}
