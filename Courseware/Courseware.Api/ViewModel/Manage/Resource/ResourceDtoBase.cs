namespace Courseware.Api.ViewModel.Manage.Resource
{
    public class ResourceDtoBase
    {
        /// <summary>
        /// id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string ResourceName { get; set; }

        /// <summary>
        /// 资源类型
        /// </summary>
        public string ResourceType { get; set; }

        /// <summary>
        /// 资源密级
        /// </summary>
        public string ResourceLevel { get; set; }

        /// <summary>
        /// 知识点
        /// </summary>
        public string ResourceTagsDisplay { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public string FileSizeDisplay { get; set; }

        /// <summary>
        /// 资源全路径
        /// </summary>
        public string ResourceUrl { get; set; }

        /// <summary>
        /// 组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 文件转换后的路径
        /// </summary>
        public string TransformUrl { get; set; }

        /// <summary>
        /// 文件后缀名
        /// </summary>
        public string FileSuffix { get; set; }

        /// <summary>
        /// 文件转换状态
        /// </summary>
        public string TransfType { get; set; } = "1";
    }
}
