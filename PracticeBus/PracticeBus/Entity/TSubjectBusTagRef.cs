//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//     Website: http://www.freesql.net
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace PracticeBus.Entity {

	[JsonObject(MemberSerialization.OptIn), Table(Name = "t_subject_bus_tag_ref")]
	public partial class TSubjectBusTagRef {

		/// <summary>
		/// 逻辑删除
		/// </summary>
		[JsonProperty, Column(Name = "delete_flag")]
		public sbyte DeleteFlag { get; set; }

		/// <summary>
		/// 科目id
		/// </summary>
		[JsonProperty, Column(Name = "subject_id")]
		public long SubjectBus_id { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[JsonProperty, Column(Name = "t_create", ServerTime = DateTimeKind.Local, CanUpdate = false)]
		public DateTime TCreate { get; set; }

		/// <summary>
		/// 修改时间
		/// </summary>
		[JsonProperty, Column(Name = "t_modified", ServerTime = DateTimeKind.Local)]
		public DateTime? TModified { get; set; }

		/// <summary>
		/// 知识点id
		/// </summary>
		[JsonProperty, Column(Name = "tag_id")]
		public long KnowledgeTag_id { get; set; }

	}

}
