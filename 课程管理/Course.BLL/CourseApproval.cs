using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Course.Model;

namespace Course.BLL
{
    public class CourseApproval
    {
        /// <summary>
        /// 已审核
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <returns></returns>
        public object GetCourseAboutApprovaled(pf_course_manage_v1Context db, Query queryCriteria)
        {
            try
            {
                if (queryCriteria.IsAsc && !string.IsNullOrEmpty(queryCriteria.FieldName))
                {
                    IQueryable<t_course> query1 = from c in db.t_course
                                                  join k in db.t_course_know_tag on c.id equals k.course_id into ck
                                                  from _ck in ck.DefaultIfEmpty()

                                                  join t in db.t_knowledge_tag on _ck.tag_id equals t.id into ct
                                                  from _ct in ct.DefaultIfEmpty()

                                                  where c.delete_flag == 0
                                                        && (c.approval_status == "3" || c.approval_status == "4")
                                                        && (string.IsNullOrEmpty(queryCriteria.CourseName) ? true : c.course_name.Contains(queryCriteria.CourseName))
                                                  orderby PubMethod.GetPropertyValue(c, queryCriteria.FieldName)
                                                  select c;

                    var count = query1.Distinct().Count();
                    List<t_course> q = query1.Distinct().Skip(queryCriteria.PageSize * (queryCriteria.PageIndex - 1)).Take(queryCriteria.PageSize).ToList();
                    if (q.Count > 0)
                    {
                        List<Course_TagList> list = new List<Course_TagList>();
                        for (int i = 0; i < q.Count; i++)
                        {
                            var qq = from k in db.t_course_know_tag
                                     join t in db.t_knowledge_tag on k.tag_id equals t.id
                                     where k.course_id == q[i].id
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
                            list.Add(new Course_TagList
                            {
                                ID = q[i].id,
                                CourseName = q[i].course_name,
                                CourseCount = q[i].course_count,
                                CreateTime = q[i].approval_date,
                                CreateName = q[i].user_name,
                                ApprovalUserNumber = q[i].approval_user_name,
                                CourseConfidential = q[i].course_confidential,
                                ApprovalStatus = q[i].approval_status,
                                ApprovalRemark = q[i].approval_remarks,
                                Tag = tags
                            });
                        }
                        return new { code = 200, result = new { count = count, list = list }, message = "OK" };
                    }
                    else
                        return new { code = 200, message = "OK" };
                }
                else if (queryCriteria.IsAsc == false && !string.IsNullOrEmpty(queryCriteria.FieldName))
                {
                    IQueryable<t_course> query1 = from c in db.t_course
                                                  join k in db.t_course_know_tag on c.id equals k.course_id into ck
                                                  from _ck in ck.DefaultIfEmpty()

                                                  join t in db.t_knowledge_tag on _ck.tag_id equals t.id into ct
                                                  from _ct in ct.DefaultIfEmpty()

                                                  where c.delete_flag == 0
                                                        && (c.approval_status == "3" || c.approval_status == "4")
                                                        && (string.IsNullOrEmpty(queryCriteria.CourseName) ? true : c.course_name.Contains(queryCriteria.CourseName))
                                                  orderby PubMethod.GetPropertyValue(c, queryCriteria.FieldName) descending
                                                  select c;


                    var count = query1.Distinct().Count();
                    List<t_course> q = query1.Distinct().Skip(queryCriteria.PageSize * (queryCriteria.PageIndex - 1)).Take(queryCriteria.PageSize).ToList();
                    if (q.Count > 0)
                    {
                        List<Course_TagList> list = new List<Course_TagList>();
                        for (int i = 0; i < q.Count; i++)
                        {
                            var qq = from k in db.t_course_know_tag
                                     join t in db.t_knowledge_tag on k.tag_id equals t.id
                                     where k.course_id == q[i].id
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
                            list.Add(new Course_TagList
                            {
                                ID = q[i].id,
                                CourseName = q[i].course_name,
                                CourseCount = q[i].course_count,
                                ApprovalUserNumber = q[i].approval_user_name,
                                CreateTime = q[i].approval_date,
                                CreateName = q[i].user_name,
                                CourseConfidential = q[i].course_confidential,
                                ApprovalStatus = q[i].approval_status,
                                ApprovalRemark = q[i].approval_remarks,
                                Tag = tags
                            });
                        }
                        return new { code = 200, result = new { count = count, list = list }, message = "OK" };
                    }
                    else
                        return new { code = 200, message = "OK" };
                }
                else
                {
                    IQueryable<t_course> query1 = from c in db.t_course
                                                  join k in db.t_course_know_tag on c.id equals k.course_id into ck
                                                  from _ck in ck.DefaultIfEmpty()

                                                  join t in db.t_knowledge_tag on _ck.tag_id equals t.id into ct
                                                  from _ct in ct.DefaultIfEmpty()

                                                  where c.delete_flag == 0
                                                        && (c.approval_status == "3" || c.approval_status == "4")
                                                        && (string.IsNullOrEmpty(queryCriteria.CourseName) ? true : c.course_name.Contains(queryCriteria.CourseName))
                                                  orderby c.approval_date descending
                                                  select c;
                    var count = query1.Distinct().Count();//计算查询数量
                    List<t_course> q = query1.Distinct().Skip(queryCriteria.PageSize * (queryCriteria.PageIndex - 1)).Take(queryCriteria.PageSize).ToList();//分页处理
                    if (q.Count > 0)
                    {
                        List<Course_TagList> list = new List<Course_TagList>();
                        for (int i = 0; i < q.Count; i++)
                        {
                            //查询知识点标签
                            var qq = from k in db.t_course_know_tag
                                     join t in db.t_knowledge_tag on k.tag_id equals t.id
                                     where k.course_id == q[i].id
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
                            list.Add(new Course_TagList
                            {
                                ID = q[i].id,
                                CourseName = q[i].course_name,
                                CourseCount = q[i].course_count,
                                ApprovalUserNumber = q[i].approval_user_name,
                                CourseConfidential = q[i].course_confidential,
                                CreateTime = q[i].approval_date,
                                CreateName = q[i].user_name,
                                ApprovalStatus = q[i].approval_status,
                                ApprovalRemark = q[i].approval_remarks,
                                Tag = tags
                            });
                        }
                        return new { code = 200, result = new { count = count, list = list }, message = "OK" };
                    }
                    else
                        return new { code = 200, message = "OK" };

                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        /// <summary>
        ///未审核 
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <returns></returns>
        public object GetCourseAboutNotApprovaled(pf_course_manage_v1Context db, Query queryCriteria)
        {
            try
            {
                if (queryCriteria.IsAsc && !string.IsNullOrEmpty(queryCriteria.FieldName))
                {
                    IQueryable<t_course> query1 = from c in db.t_course
                                                  join k in db.t_course_know_tag on c.id equals k.course_id into ck
                                                  from _ck in ck.DefaultIfEmpty()

                                                  join t in db.t_knowledge_tag on _ck.tag_id equals t.id into ct
                                                  from _ct in ct.DefaultIfEmpty()

                                                  where c.delete_flag == 0
                                                        && c.approval_status == "2"
                                                        && (string.IsNullOrEmpty(queryCriteria.CourseName) ? true : c.course_name.Contains(queryCriteria.CourseName))
                                                  orderby PubMethod.GetPropertyValue(c, queryCriteria.FieldName)
                                                  select c;

                    var count = query1.Distinct().Count();
                    List<t_course> q = query1.Distinct().Skip(queryCriteria.PageSize * (queryCriteria.PageIndex - 1)).Take(queryCriteria.PageSize).ToList();
                    if (q.Count > 0)
                    {
                        List<Course_TagList> list = new List<Course_TagList>();
                        for (int i = 0; i < q.Count; i++)
                        {
                            var qq = from k in db.t_course_know_tag
                                     join t in db.t_knowledge_tag on k.tag_id equals t.id
                                     where k.course_id == q[i].id
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
                            list.Add(new Course_TagList
                            {
                                ID = q[i].id,
                                CourseName = q[i].course_name,
                                CourseCount = q[i].course_count,
                                CreateTime = q[i].create_time,
                                CreateName = q[i].user_name,
                                ApprovalUserNumber = q[i].approval_user_number,
                                ApprovalStatus = q[i].approval_status,
                                CourseConfidential = q[i].course_confidential,
                                ApprovalRemark = q[i].approval_remarks,
                                Tag = tags
                            });
                        }
                        return new { code = 200, result = new { count = count, list = list }, message = "OK" };
                    }
                    else
                        return new { code = 200, message = "OK" };
                }
                else if (queryCriteria.IsAsc == false && !string.IsNullOrEmpty(queryCriteria.FieldName))
                {
                    IQueryable<t_course> query1 = from c in db.t_course
                                                  join k in db.t_course_know_tag on c.id equals k.course_id into ck
                                                  from _ck in ck.DefaultIfEmpty()

                                                  join t in db.t_knowledge_tag on _ck.tag_id equals t.id into ct
                                                  from _ct in ct.DefaultIfEmpty()

                                                  where c.delete_flag == 0
                                                        && c.approval_status == "2"
                                                        && (string.IsNullOrEmpty(queryCriteria.CourseName) ? true : c.course_name.Contains(queryCriteria.CourseName))
                                                  orderby PubMethod.GetPropertyValue(c, queryCriteria.FieldName) descending
                                                  select c;


                    var count = query1.Distinct().Count();
                    List<t_course> q = query1.Distinct().Skip(queryCriteria.PageSize * (queryCriteria.PageIndex - 1)).Take(queryCriteria.PageSize).ToList();
                    if (q.Count > 0)
                    {
                        List<Course_TagList> list = new List<Course_TagList>();
                        for (int i = 0; i < q.Count; i++)
                        {
                            var qq = from k in db.t_course_know_tag
                                     join t in db.t_knowledge_tag on k.tag_id equals t.id
                                     where k.course_id == q[i].id
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
                            list.Add(new Course_TagList
                            {
                                ID = q[i].id,
                                CourseName = q[i].course_name,
                                CourseCount = q[i].course_count,
                                ApprovalUserNumber = q[i].approval_user_number,
                                CreateTime = q[i].create_time,
                                CreateName = q[i].user_name,
                                CourseConfidential = q[i].course_confidential,
                                ApprovalStatus = q[i].approval_status,
                                ApprovalRemark = q[i].approval_remarks,
                                Tag = tags
                            });
                        }
                        return new { code = 200, result = new { count = count, list = list }, message = "OK" };
                    }
                    else
                        return new { code = 200, message = "OK" };
                }
                else
                {
                    IQueryable<t_course> query1 = from c in db.t_course
                                                  join k in db.t_course_know_tag on c.id equals k.course_id into ck
                                                  from _ck in ck.DefaultIfEmpty()

                                                  join t in db.t_knowledge_tag on _ck.tag_id equals t.id into ct
                                                  from _ct in ct.DefaultIfEmpty()

                                                  where c.delete_flag == 0
                                                        && c.approval_status == "2"
                                                        && (string.IsNullOrEmpty(queryCriteria.CourseName) ? true : c.course_name.Contains(queryCriteria.CourseName))
                                                  orderby c.create_time descending
                                                  select c;


                    var count = query1.Distinct().Count();//计算查询数量
                    List<t_course> q = query1.Distinct().Skip(queryCriteria.PageSize * (queryCriteria.PageIndex - 1)).Take(queryCriteria.PageSize).ToList();//分页处理
                    if (q.Count > 0)
                    {
                        List<Course_TagList> list = new List<Course_TagList>();
                        for (int i = 0; i < q.Count; i++)
                        {
                            //查询知识点标签
                            var qq = from k in db.t_course_know_tag
                                     join t in db.t_knowledge_tag on k.tag_id equals t.id
                                     where k.course_id == q[i].id
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
                            list.Add(new Course_TagList
                            {
                                ID = q[i].id,
                                CourseName = q[i].course_name,
                                CourseCount = q[i].course_count,
                                ApprovalUserNumber = q[i].approval_user_number,
                                CreateTime = q[i].create_time,
                                CreateName = q[i].user_name,
                                CourseConfidential = q[i].course_confidential,
                                ApprovalStatus = q[i].approval_status,
                                ApprovalRemark = q[i].approval_remarks,
                                Tag = tags
                            });
                        }
                        return new { code = 200, result = new { count = count, list = list }, message = "OK" };
                    }
                    else
                        return new { code = 200, message = "OK" };
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public object Update_CourseApprovalByID(pf_course_manage_v1Context db,RabbitMQClient rabbit, Approval approval, TokenModel token)
        {
            try
            {
                var query = from c in db.t_course
                            where c.delete_flag == 0 && c.id == approval.courseID
                            select c;
                var q = query.FirstOrDefault();
                q.approval_date = DateTime.Now;
                q.approval_remarks = approval.approvalRemark;
                q.approval_status = approval.approvalResult;
                q.approval_user_id = approval.approvalUserID;
                q.approval_user_number = approval.approvalUserNumber;
                q.approval_user_name = token.userName;
                db.SaveChanges();

                //审核消息推送
                MsgToDo model = new MsgToDo();
                model.todoType = 1;
                model.commonId = q.id;
                model.pubTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                model.msgName = "您有一个课程已审核";

                string strResult = string.Empty;
                if (approval.approvalResult == "3")
                    strResult = "通过";
                else
                    strResult = "拒绝";

                model.msgBody = "课程名：" + q.course_name + ",已审核，结果为：" + strResult + "！";
                model.finishFlag = 2;
                rabbit.ToDoMsg(model);
                //PubMethod.CourseApproval(model);

                SysLogModel syslog = new SysLogModel();
                syslog.opNo = token.userNumber;
                syslog.opName = token.userName;
                syslog.moduleName = "课程审核";
                syslog.opType = 3;
                syslog.logDesc = "审核课程：" + q.course_name + "，结果为：" + strResult;
                syslog.logSuccessd = 1;
                rabbit.LogMsg(syslog);
               // PubMethod.Log(syslog);
                return new { code = 200, message = "OK" };


            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        public object Update_CourseApprovalBatch(pf_course_manage_v1Context db,RabbitMQClient rabbit, ApprovalResult approvalResult, TokenModel token)
        {
            try
            {
                if (approvalResult.list.Count > 0)
                {
                    string courseName = "";
                    for (int i = 0; i < approvalResult.list.Count; i++)
                    {
                        var query = from c in db.t_course
                                    where c.id == approvalResult.list[i]
                                    select c;
                        var q = query.FirstOrDefault();
                        q.approval_status = approvalResult.ApprovalCode;
                        q.approval_date = DateTime.Now;
                        q.approval_user_id = approvalResult.UserID;
                        q.approval_user_number = approvalResult.approvalUserNumber;
                        q.approval_user_name = token.userName;
                        courseName = courseName + "," + q.course_name;
                    }
                    db.SaveChanges();
                    if (approvalResult.list.Count > 0)
                    {
                        for (int i = 0; i < approvalResult.list.Count; i++)
                        {
                            //审核消息推送
                            MsgToDo model = new MsgToDo();
                            model.todoType = 1;
                            model.commonId = approvalResult.list[i];
                            model.pubTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            model.msgName = "审核人员已批量审核了您的课程";

                            string strResult = string.Empty;
                            if (approvalResult.ApprovalCode == "3")
                                strResult = "通过";
                            else
                                strResult = "拒绝";

                            model.msgBody = "批量审核，结果为：" + strResult + "！";
                            model.finishFlag = 2;
                            rabbit.ToDoMsg(model);
                        }
                    }
                    courseName = courseName.TrimStart(',');
                    SysLogModel syslog = new SysLogModel();
                    syslog.opNo = approvalResult.approvalUserNumber;
                    syslog.opName = token.userName;
                    syslog.moduleName = "课程审核";
                    syslog.opType = 3;
                    syslog.logDesc = "批量审核课程:" + courseName + "。结果为：" + approvalResult.ApprovalCode;
                    syslog.logSuccessd = 1;
                    rabbit.LogMsg(syslog);
                    return new { code = 200, message = "OK" };
                }
                else
                {
                    SysLogModel syslog = new SysLogModel();
                    syslog.opNo = token.userNumber;
                    syslog.opName = token.userName;
                    syslog.opType = 3;
                    syslog.logDesc = "批量审核课程，结果为：" + approvalResult.ApprovalCode;
                    syslog.logSuccessd = 2;
                    rabbit.LogMsg(syslog);
                    return new { code = 400, message = "Error" };
                }

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        public object GetApprovalCourse(pf_course_manage_v1Context db, QueryCriteria queryCriteria)
        {
            try
            {
                if (queryCriteria.IsAsc && !string.IsNullOrEmpty(queryCriteria.FieldName))
                {
                    var query = from c in db.t_course
                                join k in db.t_course_know_tag on c.id equals k.course_id into ck
                                from _ck in ck.DefaultIfEmpty()

                                join t in db.t_knowledge_tag on _ck.tag_id equals t.id into ct
                                from _ct in ct.DefaultIfEmpty()

                                where c.delete_flag == 0 && c.approval_status == "3"
                                      && (queryCriteria.UserID == 0 ? true : c.create_by == queryCriteria.UserID)
                                      && ((string.IsNullOrEmpty(queryCriteria.CourseNameTag) ? true : c.course_name.Contains(queryCriteria.CourseNameTag))
                                      || (string.IsNullOrEmpty(queryCriteria.CourseNameTag) ? true : _ct.tag.Contains(queryCriteria.CourseNameTag)))
                                      && (queryCriteria.TagID == 0 ? true : _ct.src_id == queryCriteria.TagID)
                                orderby PubMethod.GetPropertyValue(c, queryCriteria.FieldName)
                                select c;
                    var count = query.Distinct().ToList().Count;
                    List<t_course> q = query.Distinct().Skip(queryCriteria.PageSize * (queryCriteria.PageIndex - 1)).Take(queryCriteria.PageSize).ToList();
                    if (q.Count > 0)
                    {
                        List<Course_TagList> list = new List<Course_TagList>();
                        for (int i = 0; i < q.Count; i++)
                        {
                            var qq = from k in db.t_course_know_tag
                                     join t in db.t_knowledge_tag on k.tag_id equals t.id
                                     where k.course_id == q[i].id
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
                            list.Add(new Course_TagList
                            {
                                ID = q[i].id,
                                CourseName = q[i].course_name,
                                CourseDesc = q[i].course_desc,
                                CourseCount = q[i].course_count,
                                ImagePath = q[i].thumbnail_path,
                                CreateTime = q[i].create_time,
                                CreateName = q[i].user_name,
                                ApprovalStatus = q[i].approval_status,
                                CourseConfidential = q[i].course_confidential,
                                ApprovalUserNumber = q[i].approval_user_number,
                                Tag = tags
                            });
                        }
                        return new { code = 200, result = new { count = count, list = list }, message = "OK" };
                    }
                    else
                        return new { code = 200, message = "OK" };
                }
                else if (queryCriteria.IsAsc == false && !string.IsNullOrEmpty(queryCriteria.FieldName))
                {
                    var query = from c in db.t_course
                                join k in db.t_course_know_tag on c.id equals k.course_id into ck
                                from _ck in ck.DefaultIfEmpty()

                                join t in db.t_knowledge_tag on _ck.tag_id equals t.id into ct
                                from _ct in ct.DefaultIfEmpty()

                                where c.delete_flag == 0 && c.approval_status == "3"
                                      && (queryCriteria.UserID == 0 ? true : c.create_by == queryCriteria.UserID)
                                      && ((string.IsNullOrEmpty(queryCriteria.CourseNameTag) ? true : c.course_name.Contains(queryCriteria.CourseNameTag))
                                      || (string.IsNullOrEmpty(queryCriteria.CourseNameTag) ? true : _ct.tag.Contains(queryCriteria.CourseNameTag)))
                                      && (queryCriteria.TagID == 0 ? true : _ct.src_id == queryCriteria.TagID)
                                orderby PubMethod.GetPropertyValue(c, queryCriteria.FieldName) descending
                                select c;
                    var count = query.Distinct().ToList().Count;
                    List<t_course> q = query.Distinct().Skip(queryCriteria.PageSize * (queryCriteria.PageIndex - 1)).Take(queryCriteria.PageSize).ToList();
                    if (q.Count > 0)
                    {
                        List<Course_TagList> list = new List<Course_TagList>();
                        for (int i = 0; i < q.Count; i++)
                        {
                            var qq = from k in db.t_course_know_tag
                                     join t in db.t_knowledge_tag on k.tag_id equals t.id
                                     where k.course_id == q[i].id
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
                            list.Add(new Course_TagList
                            {
                                ID = q[i].id,
                                CourseName = q[i].course_name,
                                CourseDesc = q[i].course_desc,
                                ImagePath = q[i].thumbnail_path,
                                CourseCount = q[i].course_count,
                                CreateTime = q[i].create_time,
                                CreateName = q[i].user_name,
                                CourseConfidential = q[i].course_confidential,
                                ApprovalStatus = q[i].approval_status,
                                ApprovalUserNumber = q[i].approval_user_number,
                                Tag = tags
                            });
                        }
                        return new { code = 200, result = new { count = count, list = list }, message = "OK" };
                    }
                    else
                        return new { code = 200, message = "OK" };
                }
                else
                {
                    var query = from c in db.t_course
                                join k in db.t_course_know_tag on c.id equals k.course_id into ck
                                from _ck in ck.DefaultIfEmpty()

                                join t in db.t_knowledge_tag on _ck.tag_id equals t.id into ct
                                from _ct in ct.DefaultIfEmpty()

                                where c.delete_flag == 0 && c.approval_status == "3"
                                      && (queryCriteria.UserID == 0 ? true : c.create_by == queryCriteria.UserID)
                                      && ((string.IsNullOrEmpty(queryCriteria.CourseNameTag) ? true : c.course_name.Contains(queryCriteria.CourseNameTag))
                                      || (string.IsNullOrEmpty(queryCriteria.CourseNameTag) ? true : _ct.tag.Contains(queryCriteria.CourseNameTag)))
                                      && (queryCriteria.TagID == 0 ? true : _ct.src_id == queryCriteria.TagID)
                                orderby c.create_time descending
                                select c;
                    var count = query.Distinct().ToList().Count;
                    List<t_course> q = query.Distinct().Skip(queryCriteria.PageSize * (queryCriteria.PageIndex - 1)).Take(queryCriteria.PageSize).ToList();
                    if (q.Count > 0)
                    {
                        List<Course_TagList> list = new List<Course_TagList>();
                        for (int i = 0; i < q.Count; i++)
                        {
                            var qq = from k in db.t_course_know_tag
                                     join t in db.t_knowledge_tag on k.tag_id equals t.id
                                     where k.course_id == q[i].id
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
                            list.Add(new Course_TagList
                            {
                                ID = q[i].id,
                                CourseName = q[i].course_name,
                                CourseDesc = q[i].course_desc,
                                ImagePath = q[i].thumbnail_path,
                                CourseCount = q[i].course_count,
                                CreateTime = q[i].create_time,
                                CreateName = q[i].user_name,
                                CourseConfidential = q[i].course_confidential,
                                ApprovalStatus = q[i].approval_status,
                                ApprovalUserNumber = q[i].approval_user_number,
                                Tag = tags
                            });
                        }
                        return new { code = 200, result = new { count = count, list = list }, message = "OK" };
                    }
                    else
                        return new { code = 200, message = "OK" };
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

    }

    public class ApprovalResult
    {
        public List<long> list { get; set; }
        public long UserID { get; set; }
        public string ApprovalCode { get; set; }
        public string approvalUserNumber { get; set; }
    }

    public class Approval
    {
        public long courseID { get; set; }
        public string approvalResult { get; set; }
        public long approvalUserID { get; set; }
        public string approvalUserNumber { get; set; }
        public string approvalRemark { get; set; }
    }

    public class Query
    {
        public string CourseName { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string FieldName { get; set; }
        public bool IsAsc { get; set; }

    }
}
