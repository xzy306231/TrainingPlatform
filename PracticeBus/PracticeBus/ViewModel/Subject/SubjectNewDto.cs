using System.Collections.Generic;
using Newtonsoft.Json;

namespace PracticeBus.ViewModel.Subject
{
    public class SubjectNewDto :BaseSubjectDto
    {
        /// <summary>
        /// 资源库中id
        /// </summary>
        [JsonProperty("id")]
        public long OriginalId { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public int Version { get; set; }
        
        public List<TagDto> Tags { get; set; }
    }
}
