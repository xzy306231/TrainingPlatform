using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using TrainSubject.Model;

namespace TrainSubject.BLL
{
    public class TrainSubjectManagement
    {
        public object GetTrainSubject(QueryCriteria queryCriteria)
        {
            try
            {
                using (var db = new pf_train_subjectContext())
                {
                    if (queryCriteria.IsAsc && !string.IsNullOrEmpty(queryCriteria.FieldName))//升序
                    {
                        var query = from t in db.t_train_subject
                                    join k in db.t_subject_know_tag on t.id equals k.subject_id into tk
                                    from _tk in tk.DefaultIfEmpty()

                                    join o in db.t_knowledge_tag on _tk.tag_id equals o.id into ok
                                    from _ok in ok.DefaultIfEmpty()

                                    where t.delete_flag == 0
                                    && (queryCriteria.planeType == "全部" ? true : t.plane_type == queryCriteria.planeType)
                                    && (string.IsNullOrEmpty(queryCriteria.TrainName) ? true : t.train_name.Contains(queryCriteria.TrainName))
                                    && (queryCriteria.tagid == 0 ? true : _ok.src_id == queryCriteria.tagid)
                                    orderby PubMethod.GetPropertyValue(t, queryCriteria.FieldName)
                                    select t;
                        var count = query.Distinct().ToList().Count;
                        List<t_train_subject> q = query.Distinct().Skip(queryCriteria.pagesize * (queryCriteria.pageindex - 1)).Take(queryCriteria.pagesize).ToList();
                        if (q.Count > 0)
                        {
                            List<Subject_TagList> list = new List<Subject_TagList>();
                            for (int i = 0; i < q.Count; i++)
                            {
                                var qq = from k in db.t_subject_know_tag
                                         join t in db.t_knowledge_tag on k.tag_id equals t.id
                                         where k.subject_id == q[i].id
                                         select new { t.src_id, t.tag };
                                var qqList = qq.ToList();
                                List<KnowledgeTag> tags = new List<KnowledgeTag>();
                                if (qqList.Count > 0)
                                {
                                    for (int j = 0; j < qqList.Count; j++)
                                    {
                                        tags.Add(new KnowledgeTag() { ID = qqList[j].src_id, TagName = qqList[j].tag });
                                    }
                                }
                                list.Add(new Subject_TagList
                                {
                                    id = q[i].id,
                                    train_name = q[i].train_name,
                                    train_number = q[i].train_number,
                                    trainDesc = q[i].train_desc,
                                    train_kind = q[i].train_kind,
                                    plane_type = q[i].plane_type,
                                    create_by = q[i].create_by,
                                    CreateName = q[i].create_name,
                                    create_time = q[i].create_time,
                                    expectResult = q[i].expect_result,
                                    planeKey = q[i].plane_type_key,
                                    Tag = tags
                                });
                            }
                            return new { code = 200, result = new { count = count, list = list }, msg = "OK" };
                        }
                        else
                            return new { code = 200, msg = "OK" };
                    }
                    else if (queryCriteria.IsAsc == false && !string.IsNullOrEmpty(queryCriteria.FieldName))
                    {
                        var query = from t in db.t_train_subject
                                    join k in db.t_subject_know_tag on t.id equals k.subject_id into tk
                                    from _tk in tk.DefaultIfEmpty()

                                    join o in db.t_knowledge_tag on _tk.tag_id equals o.id into ok
                                    from _ok in ok.DefaultIfEmpty()

                                    where t.delete_flag == 0
                                     && (queryCriteria.planeType == "全部" ? true : t.plane_type == queryCriteria.planeType)
                                    && (string.IsNullOrEmpty(queryCriteria.TrainName) ? true : t.train_name.Contains(queryCriteria.TrainName))
                                    && (queryCriteria.tagid == 0 ? true : _ok.src_id == queryCriteria.tagid)
                                    orderby PubMethod.GetPropertyValue(t, queryCriteria.FieldName) descending
                                    select t;
                        var count = query.Distinct().ToList().Count;
                        List<t_train_subject> q = query.Distinct().Skip(queryCriteria.pagesize * (queryCriteria.pageindex - 1)).Take(queryCriteria.pagesize).ToList();
                        if (q.Count > 0)
                        {
                            List<Subject_TagList> list = new List<Subject_TagList>();
                            for (int i = 0; i < q.Count; i++)
                            {
                                var qq = from k in db.t_subject_know_tag
                                         join t in db.t_knowledge_tag on k.tag_id equals t.id
                                         where k.subject_id == q[i].id
                                         select new { t.src_id, t.tag };
                                var qqList = qq.ToList();
                                List<KnowledgeTag> tags = new List<KnowledgeTag>();
                                if (qqList.Count > 0)
                                {
                                    for (int j = 0; j < qqList.Count; j++)
                                    {
                                        tags.Add(new KnowledgeTag() { ID = qqList[j].src_id, TagName = qqList[j].tag });
                                    }
                                }
                                list.Add(new Subject_TagList
                                {
                                    id = q[i].id,
                                    train_name = q[i].train_name,
                                    train_number = q[i].train_number,
                                    trainDesc = q[i].train_desc,
                                    train_kind = q[i].train_kind,
                                    plane_type = q[i].plane_type,
                                    CreateName = q[i].create_name,
                                    create_by = q[i].create_by,
                                    create_time = q[i].create_time,
                                    expectResult = q[i].expect_result,
                                    planeKey = q[i].plane_type_key,
                                    Tag = tags
                                });
                            }
                            return new { code = 200, result = new { count = count, list = list }, msg = "OK" };
                        }
                        else
                            return new { code = 200, msg = "OK" };
                    }
                    else//不排序
                    {
                        var query = from t in db.t_train_subject
                                    join k in db.t_subject_know_tag on t.id equals k.subject_id into tk
                                    from _tk in tk.DefaultIfEmpty()

                                    join o in db.t_knowledge_tag on _tk.tag_id equals o.id into ok
                                    from _ok in ok.DefaultIfEmpty()

                                    where t.delete_flag == 0
                                     && (queryCriteria.planeType == "全部" ? true : t.plane_type == queryCriteria.planeType)
                                    && (string.IsNullOrEmpty(queryCriteria.TrainName) ? true : t.train_name.Contains(queryCriteria.TrainName))
                                    && (queryCriteria.tagid == 0 ? true : _ok.src_id == queryCriteria.tagid)
                                    orderby t.create_time descending
                                    select t;
                        var count = query.Distinct().ToList().Count;
                        List<t_train_subject> q = query.Distinct().Skip(queryCriteria.pagesize * (queryCriteria.pageindex - 1)).Take(queryCriteria.pagesize).ToList();
                        if (q.Count > 0)
                        {
                            List<Subject_TagList> list = new List<Subject_TagList>();
                            for (int i = 0; i < q.Count; i++)
                            {
                                var qq = from k in db.t_subject_know_tag
                                         join t in db.t_knowledge_tag on k.tag_id equals t.id
                                         where k.subject_id == q[i].id
                                         select new { t.src_id, t.tag };
                                var qqList = qq.ToList();
                                List<KnowledgeTag> tags = new List<KnowledgeTag>();
                                if (qqList.Count > 0)
                                {
                                    for (int j = 0; j < qqList.Count; j++)
                                    {
                                        tags.Add(new KnowledgeTag() { ID = qqList[j].src_id, TagName = qqList[j].tag });
                                    }
                                }
                                list.Add(new Subject_TagList
                                {
                                    id = q[i].id,
                                    train_name = q[i].train_name,
                                    train_number = q[i].train_number,
                                    trainDesc = q[i].train_desc,
                                    train_kind = q[i].train_kind,
                                    plane_type = q[i].plane_type,
                                    CreateName = q[i].create_name,
                                    create_by = q[i].create_by,
                                    create_time = q[i].create_time,
                                    expectResult = q[i].expect_result,
                                    planeKey = q[i].plane_type_key,
                                    Tag = tags
                                });
                            }
                            return new { code = 200, result = new { count = count, list = list }, msg = "OK" };
                        }
                        else
                            return new { code = 200, msg = "OK" };
                    }
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        public object CreateTrainSubject(TrainSubjectModel model, TokenModel token)
        {
            try
            {
                using (var db = new pf_train_subjectContext())
                {
                    //验证训练科目是否存在
                    var qExist = from s in db.t_train_subject
                                 where s.delete_flag == 0 && s.train_number == model.trainNumber.Trim()
                                 select new { s.id };
                    if (qExist.FirstOrDefault() != null)//存在
                    {
                        return new { code = 401, message = "训练科目编号已存在，请重新输入" };
                    }

                    int p = 0;
                    t_train_subject obj = new t_train_subject();
                    obj.train_name = model.trainName;
                    obj.train_desc = model.trainDesc;
                    obj.train_number = model.trainNumber.Trim();
                    obj.train_kind = model.trainKind;
                    obj.plane_type_key = model.PlaneKey;
                    obj.plane_type = model.planeType;
                    obj.expect_result = model.expectResult;
                    obj.create_time = DateTime.Now;
                    obj.update_time = DateTime.Now;
                    obj.delete_flag = 0;
                    obj.create_name = token.userName;
                    obj.create_by = model.createBy;
                    obj.update_by = model.createBy;
                    db.t_train_subject.Add(obj);
                    //EntityEntry<t_train_subject> en = db.Entry<t_train_subject>(model);
                    //en.State = EntityState.Added;
                    int i = db.SaveChanges();
                    //long MaxID = (from t in db.t_train_subject select t.id).Max();
                    long MaxID = obj.id;
                    if (model.TagList != null && model.TagList.Count > 0)
                    {
                        for (int j = 0; j < model.TagList.Count; j++)
                        {

                            var query = from k in db.t_knowledge_tag
                                        where k.delete_flag == 0 && k.src_id == model.TagList[j].ID
                                        select k;
                            var q = query.FirstOrDefault();
                            if (q == null)//副本不存在，则添加
                            {
                                //添加至副本
                                t_knowledge_tag tag = new t_knowledge_tag();
                                tag.src_id = model.TagList[j].ID;
                                tag.tag = model.TagList[j].TagName;
                                tag.create_by = model.createBy;
                                tag.create_time = DateTime.Now;
                                tag.update_by = model.createBy;
                                tag.update_time = DateTime.Now;
                                db.t_knowledge_tag.Add(tag);
                                db.SaveChanges();
                                // long MaxTagID = (from t in db.t_knowledge_tag select t.id).Max();
                                long MaxTagID = tag.id;

                                //写进关系表
                                t_subject_know_tag kt = new t_subject_know_tag();
                                kt.subject_id = MaxID;
                                kt.tag_id = MaxTagID;
                                db.t_subject_know_tag.Add(kt);
                            }
                            else//副本存在，则建立关系
                            {

                                //写进关系表
                                t_subject_know_tag kt = new t_subject_know_tag();
                                kt.subject_id = MaxID;
                                kt.tag_id = q.id;
                                db.t_subject_know_tag.Add(kt);
                            }
                        }
                        p = i + db.SaveChanges();
                    }
                    if (p + i > 0)
                    {
                        SysLogModel syslog = new SysLogModel();
                        syslog.opNo = token.userNumber;
                        syslog.opName = token.userName;
                        syslog.opType = 2;
                        syslog.logDesc = "创建了训练科目：" + model.trainName;
                        syslog.logSuccessd = 1;
                        PubMethod.Log(syslog);
                        return new { code = 200, result = p, message = "OK" };
                    }
                    else
                        return new { code = 400, result = p, message = "Error" };
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        public object UpdateTrainSubject(TrainSubjectModel model, TokenModel token)
        {
            try
            {
                using (var db = new pf_train_subjectContext())
                {
                    var query = from ts in db.t_train_subject
                                where ts.id == model.ID && ts.delete_flag == 0
                                select ts;
                    var q = query.FirstOrDefault();
                    q.train_name = model.trainName;
                    q.train_desc = model.trainDesc;
                    q.train_number = model.trainNumber;
                    q.train_kind = model.trainKind;
                    q.plane_type = model.planeType;
                    q.plane_type_key = model.PlaneKey;
                    q.update_time = DateTime.Now;
                    q.update_by = model.createBy;

                    //删除知识点关系表数据
                    var p = from skt in db.t_subject_know_tag
                            where skt.subject_id == model.ID
                            select skt;
                    db.t_subject_know_tag.RemoveRange(p.ToList());

                    //新增知识点关系
                    if (model.TagList.Count > 0)
                    {
                        for (int k = 0; k < model.TagList.Count; k++)
                        {
                            var query1 = from kk in db.t_knowledge_tag
                                         where kk.delete_flag == 0 && kk.src_id == model.TagList[k].ID
                                         select kk;
                            var q1 = query1.FirstOrDefault();

                            if (q1 == null)//副本不存在，则添加
                            {
                                //添加至副本
                                t_knowledge_tag tag = new t_knowledge_tag();
                                tag.src_id = model.TagList[k].ID;
                                tag.tag = model.TagList[k].TagName;
                                tag.create_by = model.createBy;
                                tag.create_time = DateTime.Now;
                                tag.update_by = model.createBy;
                                tag.update_time = DateTime.Now;
                                db.t_knowledge_tag.Add(tag);
                                db.SaveChanges();
                                //long MaxTagID = (from t in db.t_knowledge_tag select t.id).Max();
                                long MaxTagID = tag.id;

                                //写进关系表
                                t_subject_know_tag kt = new t_subject_know_tag();
                                kt.subject_id = model.ID;
                                kt.tag_id = MaxTagID;
                                db.t_subject_know_tag.Add(kt);
                            }
                            else//副本存在，则建立关系
                            {

                                //写进关系表
                                t_subject_know_tag kt = new t_subject_know_tag();
                                kt.subject_id = model.ID;
                                kt.tag_id = q1.id;
                                db.t_subject_know_tag.Add(kt);
                            }
                        }
                    }
                    int i = db.SaveChanges();
                    if (i > 0)
                    {
                        SysLogModel syslog = new SysLogModel();
                        syslog.opNo = token.userNumber;
                        syslog.opName = token.userName;
                        syslog.opType = 3;
                        syslog.logDesc = "修改了训练科目：" + model.trainName;
                        syslog.logSuccessd = 1;
                        PubMethod.Log(syslog);
                        return new { code = 200, result = i, message = "OK" };
                    }
                    else
                        return new { code = 400, result = i, message = "Error" };
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }
        public object RemoveTrainSubject(int id, TokenModel token)
        {
            try
            {
                using (var db = new pf_train_subjectContext())
                {
                    var query = from t in db.t_train_subject
                                where t.id == id
                                select t;
                    var q = query.FirstOrDefault();
                    q.delete_flag = 1;
                    int i = db.SaveChanges();
                    if (i > 0)
                    {
                        SysLogModel syslog = new SysLogModel();
                        syslog.opNo = token.userNumber;
                        syslog.opName = token.userName;
                        syslog.opType = 4;
                        syslog.logDesc = "删除了训练科目：" + q.train_name;
                        syslog.logSuccessd = 1;
                        PubMethod.Log(syslog);
                        return new { code = 200, result = i, message = "OK" };
                    }
                    else
                        return new { code = 400, result = i, message = "Error" };
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
        /// Excel导入
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <returns></returns>
        public object TrainSubjectExcelImport(string strFilePath)
        {
            try
            {
                DataTable dt = PubMethod.ReadExcelToDataTable(strFilePath, "Sheet1");
                if (dt.Rows.Count > 0)
                {
                    using (var db = new pf_train_subjectContext())
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            t_train_subject ts = new t_train_subject();
                            ts.train_name = dt.Rows[i]["训练科目"].ToString();
                            ts.train_desc = dt.Rows[i]["科目描述"].ToString();
                            ts.train_number = dt.Rows[i]["训练编号"].ToString();
                            ts.train_kind = dt.Rows[i]["训练类别"].ToString();
                            ts.plane_type = dt.Rows[i]["适用机型"].ToString();
                            ts.expect_result = dt.Rows[i]["预期结果"].ToString();
                            ts.delete_flag = 0;
                            ts.create_time = DateTime.Now;
                            ts.update_time = DateTime.Now;
                            db.t_train_subject.Add(ts);
                        }
                        File.Delete(strFilePath);//删除服务端文件
                        int j = db.SaveChanges();
                        if (j > 0)
                            return new { code = 200, message = "OK" };
                        else
                            return new { code = 400, message = "Error" };
                    }
                }
                File.Delete(strFilePath);//删除服务端文件
                return new { code = 400, message = "文件中不存在数据哦！" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        public object TrainSubjectExcelExport()
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "excel/";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string strFullPath = Path.Combine(path, Guid.NewGuid() + ".xlsx");
                if (File.Exists(strFullPath))
                    File.Delete(strFullPath);

                List<t_train_subject> list;
                using (var db = new pf_train_subjectContext())
                {
                    var query = from t in db.t_train_subject
                                where t.delete_flag == 0
                                select t;
                    list = query.ToList();
                }
                using (var fs = new FileStream(strFullPath, FileMode.Create, FileAccess.Write))
                {
                    IWorkbook workbook = new XSSFWorkbook();

                    var sheet = workbook.CreateSheet("训练科目");
                    sheet.DefaultColumnWidth = 20;
                    sheet.ForceFormulaRecalculation = true;

                    var headFont = workbook.CreateFont();
                    headFont.IsBold = true;

                    //标题列样式
                    var headStyle = workbook.CreateCellStyle();
                    headStyle.Alignment = HorizontalAlignment.Center;
                    headStyle.VerticalAlignment = VerticalAlignment.Center;
                    //headStyle.BorderBottom = BorderStyle.Thin;
                    //headStyle.BorderLeft = BorderStyle.Thin;
                    //headStyle.BorderRight = BorderStyle.Thin;
                    //headStyle.BorderTop = BorderStyle.Thin;
                    headStyle.SetFont(headFont);

                    var rowIndex = 0;
                    var row = sheet.CreateRow(rowIndex);
                    var cell = row.CreateCell(0);
                    cell.SetCellValue("科目名称");
                    cell.CellStyle = headStyle;

                    cell = row.CreateCell(1);
                    cell.SetCellValue("科目描述");
                    cell.CellStyle = headStyle;

                    cell = row.CreateCell(2);
                    cell.SetCellValue("训练编号");
                    cell.CellStyle = headStyle;

                    cell = row.CreateCell(3);
                    cell.SetCellValue("训练类别");
                    cell.CellStyle = headStyle;

                    cell = row.CreateCell(4);
                    cell.SetCellValue("适用机型");
                    cell.CellStyle = headStyle;

                    cell = row.CreateCell(5);
                    cell.SetCellValue("期望结果");
                    cell.CellStyle = headStyle;

                    cell = row.CreateCell(6);
                    cell.SetCellValue("创建时间");
                    cell.CellStyle = headStyle;

                    //单元格边框
                    //var cellStyle = workbook.CreateCellStyle();
                    //cellStyle.Alignment = HorizontalAlignment.Center;
                    //cellStyle.VerticalAlignment = VerticalAlignment.Center;
                    //cellStyle.BorderBottom = BorderStyle.Thin;
                    //cellStyle.BorderLeft = BorderStyle.Thin;
                    //cellStyle.BorderRight = BorderStyle.Thin;
                    //cellStyle.BorderTop = BorderStyle.Thin;

                    int Index = 1;
                    for (int i = 0; i < list.Count; i++)
                    {
                        var datarow = sheet.CreateRow(Index);
                        datarow.CreateCell(0).SetCellValue(list[i].train_name);
                        datarow.CreateCell(1).SetCellValue(list[i].train_desc);
                        datarow.CreateCell(2).SetCellValue(list[i].train_number.ToString());
                        datarow.CreateCell(3).SetCellValue(list[i].train_kind);
                        datarow.CreateCell(4).SetCellValue(list[i].plane_type);
                        datarow.CreateCell(5).SetCellValue(list[i].expect_result);
                        datarow.CreateCell(6).SetCellValue(list[i].create_time.ToString());
                        Index++;
                    }
                    workbook.Write(fs);
                    return strFullPath;
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

    public class TrainSubjectModel
    {
        public long ID { get; set; }
        public string trainName { get; set; }
        public string trainDesc { get; set; }
        public string trainNumber { get; set; }
        public string trainKind { get; set; }
        public string PlaneKey { get; set; }
        public string planeType { get; set; }
        public string expectResult { get; set; }
        public long createBy { get; set; }
        public List<KnowledgeTag> TagList { get; set; }
    }

    public class KnowledgeTag
    {
        public long? ID { get; set; }
        public string TagName { get; set; }
    }

    public class QueryCriteria
    {
        public string TrainName { get; set; }
        public string planeType { get; set; }
        public long tagid { get; set; }
        public int pagesize { get; set; }
        public int pageindex { get; set; }
        public string FieldName { get; set; }
        public bool IsAsc { get; set; }

    }
    public class Subject_TagList
    {
        public long id { get; set; }
        public string train_name { get; set; }
        public string train_number { get; set; }
        public string train_kind { get; set; }
        public string trainDesc { get; set; }
        public string expectResult { get; set; }
        public string planeKey { get; set; }
        public string plane_type { get; set; }
        public string CreateName { get; set; }
        public long? create_by { get; set; }
        public DateTime? create_time { get; set; }
        public List<KnowledgeTag> Tag { get; set; }
    }

}
