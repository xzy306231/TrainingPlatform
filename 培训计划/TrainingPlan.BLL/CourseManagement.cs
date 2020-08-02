using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Configuration;

public class CourseManagement
{
    public object GetCourseInfoByID(pf_training_plan_v1Context db, IConfiguration configuration, long ID)
    {
        try
        {
            string strFastDFSUrl = configuration["FastDFSUrl"];
            var query = from c in db.t_course
                        where c.delete_flag == 0 && c.id == ID
                        select c;
            var q = query.FirstOrDefault();
            var qq = from k in db.t_course_know_tag
                     join t in db.t_knowledge_tag on k.tag_id equals t.id
                     where k.course_id == ID
                     select new { t.src_id, t.tag };
            var qqList = qq.ToList();
            List<Tag> tags = new List<Tag>();
            if (qqList.Count > 0)
            {
                for (int j = 0; j < qqList.Count; j++)
                {
                    tags.Add(new Tag() { ID = qqList[j].src_id, TagName = qqList[j].tag });
                }
            }
            Course_TagList course_TagList = new Course_TagList();
            course_TagList.ID = q.id;
            course_TagList.CourseName = q.course_name;
            course_TagList.CourseDesc = q.course_desc;
            course_TagList.CreateName = q.user_name;
            course_TagList.CourseCount = q.course_count;
            course_TagList.LearningTime = q.learning_time.ToString();
            course_TagList.PartPath = q.thumbnail_path;
            course_TagList.CourseConfidential = q.course_confidential;
            if (string.IsNullOrEmpty(q.thumbnail_path))
            {
                course_TagList.ImagePath = "";
            }
            else
            {
                course_TagList.ImagePath = strFastDFSUrl + q.thumbnail_path;
            }
            course_TagList.CreateTime = q.create_time;
            course_TagList.Tag = tags;
            return new { code = 200, result = course_TagList, msg = "OK" };

        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }

    }

    private List<CourseStruct> list;
    /// <summary>
    /// 根据课程ID获取课程结构
    /// </summary>
    /// <param name="courseid"></param>
    /// <returns></returns>
    public object GetCourseStruct(pf_training_plan_v1Context db, long courseid)
    {
        try
        {
            list = new List<CourseStruct>();
            var query = from s in db.t_course_struct
                        join t in db.t_struct_resource on s.id equals t.course_struct_id into st
                        from _st in st.DefaultIfEmpty()
                        join r in db.t_course_resource on _st.course_resouce_id equals r.id into tr
                        from _tr in tr.DefaultIfEmpty()
                        where s.delete_flag == 0 && s.course_id == courseid && s.parent_id == 0
                        orderby s.create_time ascending
                        select new { s, _tr.resource_type, _tr.resource_extension, _tr.resource_confidential };
            var temp = query.ToList();
            if (temp.Count > 0)
            {
                for (int i = 0; i < temp.Count; i++)
                {
                    CourseStruct node = new CourseStruct();
                    node.ID = temp[i].s.struct_id;
                    node.NodeNama = temp[i].s.course_node_name;
                    node.NodeType = temp[i].s.node_type;
                    node.ResourceExtension = temp[i].resource_extension;
                    node.ParentID = temp[i].s.parent_id;
                    node.ResourceConfidential = temp[i].resource_confidential;
                    node.Sort = temp[i].s.node_sort;
                    node.ResourceType = temp[i].resource_type;
                    node.ResourceCount = temp[i].s.resource_count;
                    GetChildCourseStruct(db, node, courseid);
                    list.Add(node);
                }
            }
            return new { code = 200, result = list, message = "OK" };

        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }

    }

    private void GetChildCourseStruct(pf_training_plan_v1Context db, CourseStruct courseStruct, long courseid)
    {
        try
        {
            var query = from s in db.t_course_struct
                        join t in db.t_struct_resource on s.id equals t.course_struct_id into st
                        from _st in st.DefaultIfEmpty()
                        join r in db.t_course_resource on _st.course_resouce_id equals r.id into tr
                        from _tr in tr.DefaultIfEmpty()
                        where s.delete_flag == 0 && s.parent_id == courseStruct.ID && s.course_id == courseid
                        orderby s.create_time ascending
                        select new { s, _tr.resource_type, _tr.resource_extension, _tr.resource_confidential };
            var temp = query.ToList();
            if (temp.Count > 0)
            {
                for (int i = 0; i < temp.Count; i++)
                {
                    CourseStruct node = new CourseStruct();
                    node.ID = temp[i].s.struct_id;
                    node.NodeNama = temp[i].s.course_node_name;
                    node.NodeType = temp[i].s.node_type;
                    node.ResourceExtension = temp[i].resource_extension;
                    node.ResourceConfidential = temp[i].resource_confidential;
                    node.ParentID = temp[i].s.parent_id;
                    node.Sort = temp[i].s.node_sort;
                    node.ResourceType = temp[i].resource_type;
                    node.ResourceCount = temp[i].s.resource_count;
                    GetChildCourseStruct(db, node, courseid);
                    courseStruct.Children.Add(node);
                }
            }

        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
        }

    }

}
public class CourseStruct
{
    public CourseStruct()
    {
        Children = new List<CourseStruct>();
        ParentID = 0;
    }
    public long ID { get; set; }
    public string NodeNama { get; set; }
    public string NodeType { get; set; }
    public string ResourceType { get; set; }
    public string ResourceExtension { get; set; }
    public string ResourceConfidential { get; set; }
    public string LearningProcess { get; set; }
    public long ParentID { get; set; }
    public int? ResourceCount { get; set; }
    public int Sort { get; set; }
    public List<CourseStruct> Children { get; set; }
}


public class Course_TagList
{
    public long ID { get; set; }
    public string CourseName { get; set; }
    public string CourseDesc { get; set; }
    public string ImagePath { get; set; }
    public string PartPath { get; set; }
    public string CourseConfidential { get; set; }
    public decimal CourseCount { get; set; }
    public string LearningTime { get; set; }
    public DateTime? CreateTime { get; set; }
    public string CreateName { get; set; }
    public string ApprovalUserNumber { get; set; }
    public string ApprovalStatus { get; set; }
    public string ApprovalRemark { get; set; }

    public List<Tag> Tag { get; set; }
}


public class Tag
{
    public long? ID { get; set; }
    public string TagName { get; set; }
}

