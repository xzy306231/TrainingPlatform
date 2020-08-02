namespace CoursewareUploadHelper.Helper
{
    /// <summary>
    /// 知识树节点
    /// 到课程为止
    /// 再往下需要动态添加
    /// </summary>
    public struct CourseNodeInfo
    {
        private long _firstLevelId;
        /// <summary>
        /// 一级目录
        /// 机型-500A
        /// </summary>
        public long FirstLevelId
        {
            get { return _firstLevelId; }
            set { _firstLevelId = value; }
        }

        private long _secondLevelId;
        /// <summary>
        /// 二级目录
        /// 大分类 空地勤
        /// </summary>
        public long SecondLevelId
        {
            get { return _secondLevelId; }
            set { _secondLevelId = value; }
        }

        private long _thirdLevelId;
        /// <summary>
        /// 三级目录
        /// 专业
        /// </summary>
        public long ThirdLevelId
        {
            get { return _thirdLevelId; }
            set { _thirdLevelId = value; }
        }
        
        private long _fourthLevleId;
        /// <summary>
        /// 四级目录
        /// 课程
        /// </summary>
        public long FourthLevelId
        {
            get { return _fourthLevleId; }
            set { _fourthLevleId = value; }
        }

    }
}