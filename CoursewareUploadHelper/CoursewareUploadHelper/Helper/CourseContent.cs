using System.Collections.Generic;

// ReSharper disable All

namespace CoursewareUploadHelper.Helper
{
    /// <summary>
    /// 课程结构
    /// </summary>
    public class CourseContent
    {
        /// <summary>
        /// 课程名
        /// </summary>
        public string CourseName { get; set; }

        /// <summary>
        /// cbtstructure.xml的路径
        /// </summary>
        public string XmlFullPath { get; set; }

        /// <summary>
        /// 课程文件夹全路径
        /// </summary>
        public string CourseFolderPath { get; set; }

        /// <summary>
        /// 知识树节点信息
        /// </summary>
        public CourseNodeInfo TagNodeInfo { get; set; }

        /// <summary>
        /// 课程知识点
        /// </summary>
        public List<long> CourseTagInfo { get; set; }

        /// <summary>
        /// 课件知识点
        /// </summary>
        public List<long> CoursewareTagInfo { get; set; }
    }
}