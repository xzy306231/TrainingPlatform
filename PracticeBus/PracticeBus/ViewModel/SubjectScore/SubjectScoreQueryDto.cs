using Newtonsoft.Json;

namespace PracticeBus.ViewModel.SubjectScore
{
    public class SubjectScoreQueryDto
    {
        /// <summary>
        /// 科目id
        /// </summary>
        public long SubjectBusId { get; set; }

        /// <summary>
        /// 科目原始id
        /// </summary>
        public long OriginalId { get; set; }

        /// <summary>
        /// 科目名称
        /// </summary>
        public string SubjectName { get; set; }

        /// <summary>
        /// 标签展示
        /// </summary>
        public string TagDisplay { get; set; }

        /// <summary>
        /// 科目简介
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 适用机型value
        /// </summary>
        public string AirplaneValue { get; set; }

        /// <summary>
        /// 分类value
        /// </summary>
        public string ClassifyValue { get; set; }

        /// <summary>
        /// 科目结果
        /// </summary>
        public sbyte Result { get; set; }

        /// <summary>
        /// 科目状态
        /// </summary>
        public sbyte Status { get; set; }
    }
}
