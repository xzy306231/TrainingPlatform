using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Data;

namespace KnowledgeTag.BLL
{
    public class KnowledgeTagManagement
    {
        private List<LsTagTree> _TagList;
        public object GetTagTree(string TagName, pf_knowledge_tagContext db)
        {
            try
            {
                _TagList = new List<LsTagTree>();
                if (!string.IsNullOrEmpty(TagName))
                    TagName = TagName.Trim();
                var query = from t in db.t_knowledge_tag
                            where t.delete_flag == 0 && (string.IsNullOrEmpty(TagName) ? true : t.tag.Contains(TagName)) && t.parent_id == 0
                            orderby t.tag_sort ascending
                            select t;
                List<t_knowledge_tag> list = query.ToList();
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        LsTagTree node = new LsTagTree();
                        node.Tag = list[i].tag;
                        node.TagDesc = list[i].tag_desc;
                        node.Id = list[i].id;
                        node.ParentId = list[i].parent_id;
                        node.Sort = list[i].tag_sort;
                        node.CreateBy = list[i].create_by;
                        node.CreateTime = list[i].create_time;
                        node.UpdateBy = list[i].update_by;
                        node.UpdateTime = list[i].update_time;
                        node.IsDelete = list[i].delete_flag;
                        GetChildTagTree(db, node);
                        _TagList.Add(node);
                    }
                }
                return new { code = 200, result = _TagList, message = "ok" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        private void GetChildTagTree(pf_knowledge_tagContext db, LsTagTree tag)
        {
            try
            {
                var query = from t in db.t_knowledge_tag
                            where t.parent_id == tag.Id && t.delete_flag == 0
                            orderby t.tag_sort ascending
                            select t;
                List<t_knowledge_tag> list = query.ToList();
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        LsTagTree node = new LsTagTree();
                        node.Tag = list[i].tag;
                        node.TagDesc = list[i].tag_desc;
                        node.Id = list[i].id;
                        node.ParentId = list[i].parent_id;
                        node.Sort = list[i].tag_sort;
                        node.CreateBy = list[i].create_by;
                        node.CreateTime = list[i].create_time;
                        node.UpdateBy = list[i].update_by;
                        node.UpdateTime = list[i].update_time;
                        node.IsDelete = list[i].delete_flag;
                        //递归
                        GetChildTagTree(db, node);
                        tag.Children.Add(node);
                    }
                }

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                // return new { code = 400, msg = "Error" };
            }

        }
        public object GetTagCount(pf_knowledge_tagContext db)
        {
            try
            {
                var count = db.t_knowledge_tag.Where(x => x.delete_flag == 0).Count();
                return new { code = 200, result = count, message = "OK" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
        public object GetParentTag(pf_knowledge_tagContext db, string TagName)
        {
            try
            {
                if (!string.IsNullOrEmpty(TagName))
                    TagName = TagName.Trim();
                var query = from t in db.t_knowledge_tag
                            where t.delete_flag == 0 && (string.IsNullOrEmpty(TagName) ? true : t.tag.Contains(TagName)) && t.parent_id == 0
                            orderby t.tag_sort ascending
                            select t;
                List<t_knowledge_tag> list = query.ToList();
                List<LsTagTree> listTag = new List<LsTagTree>();
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        var queryTagCount = (from c in db.t_knowledge_tag
                                             where c.delete_flag == 0 && c.parent_id == list[i].id
                                             select c).Count();
                        listTag.Add(new LsTagTree
                        {
                            Id = list[i].id,
                            Tag = list[i].tag,
                            ChildTagCount = queryTagCount
                        });
                    }
                }
                return new
                {
                    code = 200,
                    result = listTag,
                    message = "OK"
                };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }
        public object GetChildrenTag(pf_knowledge_tagContext db, long id)
        {
            try
            {
                var query = from t in db.t_knowledge_tag
                            where t.parent_id == id && t.delete_flag == 0
                            orderby t.tag_sort ascending
                            select t;
                List<t_knowledge_tag> list = query.ToList();
                List<LsTagTree> listTag = new List<LsTagTree>();
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        var queryTagCount = (from c in db.t_knowledge_tag
                                             where c.delete_flag == 0 && c.parent_id == list[i].id
                                             select c).Count();
                        listTag.Add(new LsTagTree
                        {
                            Id = list[i].id,
                            Tag = list[i].tag,
                            ParentId = id,
                            ChildTagCount = queryTagCount
                        });
                    }
                }
                return new
                {
                    code = 200,
                    result = listTag,
                    message = "OK"
                };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        public object Add_ParentTag(pf_knowledge_tagContext db, RabbitMQClient rabbit, string TagName, int CreateID, int index, TokenModel obj)
        {
            try
            {
                var queryTagCount = db.t_knowledge_tag.Where(x => x.delete_flag == 0 && x.parent_id == 0).Select(x => x.tag_sort).Max();
                int? temp = queryTagCount == null ? 1 : ++queryTagCount;
                t_knowledge_tag t = new t_knowledge_tag();
                t.tag = TagName;
                t.parent_id = 0;
                t.tag_sort = temp;
                t.delete_flag = 0;
                t.create_by = CreateID;
                t.create_time = DateTime.Now;
                t.update_by = CreateID;
                t.update_time = DateTime.Now;
                db.t_knowledge_tag.Add(t);
                db.SaveChanges();

                SysLogModel syslog = new SysLogModel();
                syslog.opNo = obj.userNumber;
                syslog.opName = obj.userName;
                syslog.opType = 2;
                syslog.logDesc = "添加知识点：" + TagName;
                syslog.logSuccessd = 1;
                syslog.moduleName = "知识体系";
                rabbit.OperationLog(syslog);
                return new { code = 200, result = t, message = "ok" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        public object TreeNodeSort(pf_knowledge_tagContext db, long CurrentID, long DestinationID)
        {
            try
            {

                var query = from t in db.t_knowledge_tag
                            where t.id == CurrentID
                            select t;
                var q = query.FirstOrDefault();
                q.parent_id = DestinationID;
                q.update_time = DateTime.Now;
                int i = db.SaveChanges();
                if (i > 0)
                {
                    return new { code = 200, message = "ok" };
                }
                else
                {
                    return new { code = 400, message = "拖动失败" };
                }

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        public object Add_NodeTag(pf_knowledge_tagContext db, RabbitMQClient rabbit, string TagName, int ParentID, int CreateID, int index, TokenModel obj)
        {
            try
            {
                var queryTagCount = db.t_knowledge_tag.Where(x => x.delete_flag == 0 && x.parent_id == ParentID).Select(x => x.tag_sort).Max();
                int? temp = queryTagCount == null ? 1 : ++queryTagCount;
                t_knowledge_tag tag = new t_knowledge_tag();
                tag.tag = TagName;
                tag.parent_id = ParentID;
                tag.tag_sort = temp;
                tag.delete_flag = 0;
                tag.create_by = CreateID;
                tag.create_time = DateTime.Now;
                tag.update_by = CreateID;
                tag.update_time = DateTime.Now;
                db.t_knowledge_tag.Add(tag);
                int i = db.SaveChanges();

                SysLogModel syslog = new SysLogModel();
                syslog.opNo = obj.userNumber;
                syslog.opName = obj.userName;
                syslog.opType = 2;
                syslog.logDesc = "添加了知识点：" + TagName;
                syslog.logSuccessd = 1;
                syslog.moduleName = "知识体系";
                rabbit.OperationLog(syslog);
                return new { code = 200, result = tag, message = "ok" };


            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        public object Update_NodeTag(pf_knowledge_tagContext db, RabbitMQClient rabbit, string TagName, int NodeID, int UpdateBy, TokenModel obj)
        {
            try
            {
                var query = from t in db.t_knowledge_tag
                            where t.id == NodeID
                            select t;
                foreach (var item in query)
                {
                    item.tag = TagName;
                    item.update_by = UpdateBy;
                    item.update_time = DateTime.Now;
                    break;
                }
                int i = db.SaveChanges();
                if (i > 0)
                {
                    SysLogModel syslog = new SysLogModel();
                    syslog.opNo = obj.userNumber;
                    syslog.opName = obj.userName;
                    syslog.opType = 3;
                    syslog.logDesc = "修改了知识点：" + TagName;
                    syslog.logSuccessd = 1;
                    syslog.moduleName = "知识体系";
                    rabbit.OperationLog(syslog);
                    return new { code = 200, message = "ok" };
                }
                else
                {
                    SysLogModel syslog = new SysLogModel();
                    syslog.opNo = obj.userNumber;
                    syslog.opName = obj.userName;
                    syslog.opType = 3;
                    syslog.logDesc = "修改了知识点：" + TagName;
                    syslog.logSuccessd = 2;
                    syslog.moduleName = "知识体系";
                    rabbit.OperationLog(syslog);
                    return new { code = 400, message = "编辑失败" };
                }

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        public object Delete_Tag(pf_knowledge_tagContext db, RabbitMQClient rabbit, int ID, TokenModel obj)
        {
            try
            {

                var query = from t in db.t_knowledge_tag
                            where t.id == ID
                            select t;
                string tag = "";
                foreach (var item in query)
                {
                    item.delete_flag = 1;
                    tag = item.tag;
                    break;
                }
                int i = db.SaveChanges();
                if (i > 0)
                {
                    SysLogModel syslog = new SysLogModel();
                    syslog.opNo = obj.userNumber;
                    syslog.opName = obj.userName;
                    syslog.opType = 4;
                    syslog.logDesc = "删除了知识点：" + tag;
                    syslog.logSuccessd = 1;
                    syslog.moduleName = "知识体系";
                    rabbit.OperationLog(syslog);
                    return new { code = 200, message = "ok" };
                }
                else
                {
                    SysLogModel syslog = new SysLogModel();
                    syslog.opNo = obj.userNumber;
                    syslog.opName = obj.userName;
                    syslog.opType = 4;
                    syslog.logDesc = "删除了知识点：" + tag;
                    syslog.logSuccessd = 2;
                    syslog.moduleName = "知识体系";
                    rabbit.OperationLog(syslog);
                    return new { code = 400, message = "添加失败" };
                }

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }
        public object TreeSort(pf_knowledge_tagContext db, TagList tagList)
        {
            try
            {

                if (tagList != null && tagList.list.Count > 0)
                {
                    for (int i = 0; i < tagList.list.Count; i++)
                    {
                        var query = from t in db.t_knowledge_tag
                                    where t.delete_flag == 0 && t.id == tagList.list[i].ID
                                    select t;
                        var q = query.FirstOrDefault();
                        q.parent_id = tagList.list[i].ParentID;
                        q.tag_sort = tagList.list[i].Index;
                    }
                }
                if (db.SaveChanges() > 0)
                    return new { code = 200, message = "OK" };
                else
                    return new { code = 200, message = "Error" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        public object ImportExcel(pf_knowledge_tagContext db,string savePath)
        {
            try
            {
                DataTable dt = PubMethod.ReadExcelToDataTable(savePath, "Sheet1");
                ////四级菜单
                Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> dic = new Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>();
                if (dt.Rows.Count > 0)
                {
                    string strFirstMenuTemp = "";
                    string strSecondMenuTemp = "";
                    string strThirdMenuTemp = "";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        bool IsFirstMenuExist = true;
                        bool IsSecondMenuExist = true;
                        bool IsThirdMenuExist = true;
                        string strFirstMenu = dt.Rows[i]["FirstMenu"].ToString();
                        string strSecondMenu = dt.Rows[i]["SecondMenu"].ToString();
                        string strThirdMenu = dt.Rows[i]["ThirdMenu"].ToString();
                        string strFourthMenu = dt.Rows[i]["FourthMenu"].ToString();

                        if (string.IsNullOrEmpty(strFirstMenu))//不存在                
                            IsFirstMenuExist = false;
                        else //存在                 
                            strFirstMenuTemp = strFirstMenu;

                        if (string.IsNullOrEmpty(strSecondMenu))//不存在
                            IsSecondMenuExist = false;
                        else//存在
                            strSecondMenuTemp = strSecondMenu;

                        if (string.IsNullOrEmpty(strThirdMenu))//不存在
                            IsThirdMenuExist = false;
                        else//存在
                            strThirdMenuTemp = strThirdMenu;

                        if (!dic.ContainsKey(strFirstMenuTemp) && IsFirstMenuExist && IsSecondMenuExist && IsThirdMenuExist)
                        {
                            List<string> list = new List<string>();
                            list.Add(strFourthMenu);

                            Dictionary<string, List<string>> dic111 = new Dictionary<string, List<string>>();
                            dic111.Add(strThirdMenu, list);

                            Dictionary<string, Dictionary<string, List<string>>> dic11 = new Dictionary<string, Dictionary<string, List<string>>>();
                            dic11.Add(strSecondMenu, dic111);

                            dic.Add(strFirstMenu, dic11);
                        }
                        else if (dic.ContainsKey(strFirstMenuTemp) && IsFirstMenuExist == false && IsSecondMenuExist == false && IsThirdMenuExist == false)
                        {
                            dic[strFirstMenuTemp][strSecondMenuTemp][strThirdMenuTemp].Add(strFourthMenu);
                        }
                        else if (dic.ContainsKey(strFirstMenuTemp) && IsFirstMenuExist == false && IsSecondMenuExist == false && IsThirdMenuExist)//三级菜单变化
                        {
                            List<string> list = new List<string>();
                            list.Add(strFourthMenu);
                            dic[strFirstMenuTemp][strSecondMenuTemp].Add(strThirdMenuTemp, list);
                        }
                        else if (dic.ContainsKey(strFirstMenuTemp) && IsFirstMenuExist == false && IsSecondMenuExist && IsThirdMenuExist)//二级菜单变化
                        {
                            Dictionary<string, List<string>> dic1 = new Dictionary<string, List<string>>();
                            List<string> list = new List<string>();
                            list.Add(strFourthMenu);
                            dic1.Add(strThirdMenu, list);
                            dic[strFirstMenuTemp].Add(strSecondMenuTemp, dic1);
                        }
                    }
                }
                //删除本地文件
                File.Delete(savePath);
                if (DicToDataBase(db,dic))
                    return new { code = 200, message = "ok" };
                else
                    return new { code = 400, message = "导入失败" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }



            #region 二三级菜单
            //Dictionary<string, Dictionary<string, List<string>>> dic = new Dictionary<string, Dictionary<string, List<string>>>();
            ////三级菜单
            //if (dt.Rows.Count > 0)
            //{
            //    string strFirstMenuTemp = "";
            //    string strSecondMenuTemp = "";
            //    for (int i = 0; i < dt.Rows.Count; i++)
            //    {
            //        bool IsFirstMenuExist = true;
            //        bool IsSecondMenuExist = true;
            //        bool IsThirdMenuExist = true;
            //        string strFirstMenu = dt.Rows[i]["FirstMenu"].ToString();
            //        string strSecondMenu = dt.Rows[i]["SecondMenu"].ToString();
            //        string strThirdMenu = dt.Rows[i]["ThirdMenu"].ToString();

            //        if (string.IsNullOrEmpty(strFirstMenu))//不存在                
            //            IsFirstMenuExist = false;
            //        else //存在                 
            //            strFirstMenuTemp = strFirstMenu;

            //        if (string.IsNullOrEmpty(strSecondMenu))//不存在
            //            IsSecondMenuExist = false;
            //        else//存在
            //            strSecondMenuTemp = strSecondMenu;

            //        if (!dic.ContainsKey(strFirstMenuTemp) && IsFirstMenuExist && IsSecondMenuExist)
            //        {
            //            Dictionary<string, List<string>> dic1 = new Dictionary<string, List<string>>();
            //            dic.Add(strFirstMenu, dic1);
            //            List<string> list = new List<string>();
            //            list.Add(strThirdMenu);
            //            dic[strFirstMenu].Add(strSecondMenu, list);
            //        }
            //        else if (dic.ContainsKey(strFirstMenuTemp) && IsFirstMenuExist == false && IsSecondMenuExist == false)
            //        {
            //            dic[strFirstMenuTemp][strSecondMenuTemp].Add(strThirdMenu);
            //        }
            //        else if (dic.ContainsKey(strFirstMenuTemp) && IsFirstMenuExist == false && IsSecondMenuExist)
            //        {
            //            Dictionary<string, List<string>> dic2 = new Dictionary<string, List<string>>();
            //            List<string> list = new List<string>();
            //            list.Add(strThirdMenu);
            //            dic[strFirstMenuTemp].Add(strSecondMenuTemp, list);
            //        }
            //    }
            //}
            //return false;

            //Dictionary<string,  List<string>> dic = new Dictionary<string,  List<string>>();
            //二级菜单
            //if (dt.Rows.Count > 0)
            //{
            //    string strParentTemp = "";
            //    for (int i = 0; i < dt.Rows.Count; i++)
            //    {
            //        bool IsExist = true;
            //        string strParentTag = dt.Rows[i]["ParentTag"].ToString();

            //        if (string.IsNullOrEmpty(strParentTag))//不存在
            //        {
            //            IsExist = false;
            //        }
            //        else//存在
            //        {
            //            IsExist = true;
            //            strParentTemp = strParentTag;
            //        }

            //        string strChildTag = dt.Rows[i]["ChildTag"].ToString();

            //        if (!dic.ContainsKey(strParentTag) && IsExist)
            //        {
            //            List<string> list = new List<string>();
            //            list.Add(strChildTag);
            //            dic.Add(strParentTag, list);
            //        }
            //        else if(IsExist==false)
            //        {
            //            dic[strParentTemp].Add(strChildTag);
            //        }

            //    }
            //   return DicToDataBase(dic);
            //}
            //return false; 
            #endregion

        }

        private bool DicToDataBase(pf_knowledge_tagContext db, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> dic)
        {
            try
            {

                long a = 0;
                long b = 0;
                long c = 0;
                long sum = 0;
                foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, List<string>>>> item in dic)
                {
                    t_knowledge_tag first = new t_knowledge_tag();
                    first.tag = item.Key;
                    first.parent_id = 0;
                    first.delete_flag = 0;
                    first.create_time = DateTime.Now;
                    first.update_time = DateTime.Now;
                    first.create_by = 1;
                    db.t_knowledge_tag.Add(first);
                    db.SaveChanges();
                    sum++;
                    a = (from t in db.t_knowledge_tag select t.id).Max();


                    foreach (KeyValuePair<string, Dictionary<string, List<string>>> item1 in item.Value)
                    {
                        t_knowledge_tag second = new t_knowledge_tag();
                        second.tag = item1.Key;
                        second.parent_id = a;
                        second.delete_flag = 0;
                        second.create_time = DateTime.Now;
                        second.update_time = DateTime.Now;
                        second.create_by = 1;
                        db.t_knowledge_tag.Add(second);
                        db.SaveChanges();
                        sum++;
                        b = (from t in db.t_knowledge_tag select t.id).Max();

                        foreach (KeyValuePair<string, List<string>> item2 in item1.Value)
                        {
                            t_knowledge_tag third = new t_knowledge_tag();
                            third.tag = item2.Key;
                            third.parent_id = b;
                            third.delete_flag = 0;
                            third.create_time = DateTime.Now;
                            third.update_time = DateTime.Now;
                            third.create_by = 1;
                            db.t_knowledge_tag.Add(third);
                            db.SaveChanges();
                            sum++;
                            c = (from t in db.t_knowledge_tag select t.id).Max();

                            for (int i = 0; i < item2.Value.Count; i++)
                            {
                                t_knowledge_tag fourth = new t_knowledge_tag();
                                fourth.tag = item2.Value[i];
                                fourth.parent_id = c;
                                fourth.delete_flag = 0;
                                fourth.create_time = DateTime.Now;
                                fourth.update_time = DateTime.Now;
                                fourth.create_by = 1;
                                db.t_knowledge_tag.Add(fourth);
                                db.SaveChanges();
                                sum++;
                            }
                        }
                    }
                }
                if (sum > 0)
                    return true;
                else
                    return false;

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return false;
            }

        }
    }

    public class LsTagTree
    {
        public LsTagTree()
        {
            Children = new List<LsTagTree>();
            ParentId = 0;
        }
        public long? Id { get; set; }
        public string Tag { get; set; }
        public string TagDesc { get; set; }
        public long? ParentId { get; set; }
        public int? Sort { get; set; }
        public sbyte? IsDelete { get; set; }
        public long? CreateBy { get; set; }
        public DateTime? CreateTime { get; set; }
        public long? UpdateBy { get; set; }
        public int ChildTagCount { get; set; }
        public DateTime? UpdateTime { get; set; }
        public List<LsTagTree> Children { get; set; }
    }

    public class TagList
    {
        public List<Tag> list { get; set; }
    }
    public class Tag
    {
        public long? ID { get; set; }
        public string TagName { get; set; }
        public long? ParentID { get; set; }
        public int Index { get; set; }
    }
}
