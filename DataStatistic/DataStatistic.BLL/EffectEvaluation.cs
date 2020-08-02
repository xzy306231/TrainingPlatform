using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace DataStatistic.BLL
{
    public class EffectEvaluation
    {
        public object GetIndexContent(pf_datastatisticContext db, long planid)
        {
            try
            {
                var queryCheck = from i in db.t_index_check_status
                                 where i.delete_flag == 0 && i.plan_id == planid
                                 select i;
                if (queryCheck.Count() == 0)//不存在记录
                {
                    var queryIndex = db.t_index_setting.Where(x => x.delete_flag == 0).Select(arg => new { arg.id, arg.index_dif, arg.index_kind });
                    List<IndexContent> list = new List<IndexContent>();
                    foreach (var item in queryIndex)
                    {
                        list.Add(new IndexContent
                        {
                            ID = item.id,
                            IndexDif = item.index_dif,
                            IndexKind = item.index_kind,
                            CheckStatus = "1"
                        });
                    }
                    return new { code = 200, result = list, message = "OK" };
                }
                else//存在
                {
                    var queryCheckRef = from i in db.t_index_check_status
                                        join s in db.t_index_setting on i.index_id equals s.id
                                        where i.delete_flag == 0 && i.plan_id == planid
                                        select new { s.id, s.index_dif, s.index_kind, i.check_flag };
                    List<IndexContent> list = new List<IndexContent>();
                    foreach (var item in queryCheckRef)
                    {
                        list.Add(new IndexContent
                        {
                            ID = item.id,
                            IndexDif = item.index_dif,
                            IndexKind = item.index_kind,
                            CheckStatus = item.check_flag.ToString()
                        });
                    }
                    return new { code = 200, result = list, message = "OK" };
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }
        public object SaveIndexStatus(pf_datastatisticContext db, List<IndexContent> indexContents)
        {
            try
            {
                if (indexContents != null && indexContents.Count > 0)
                {
                    var queryCheck = from c in db.t_index_check_status
                                     where c.delete_flag == 0 && c.plan_id == indexContents[0].PlanID
                                     select c;
                    //删除
                    foreach (var item in queryCheck)
                    {
                        item.delete_flag = 1;
                    }
                    //新增
                    for (int i = 0; i < indexContents.Count; i++)
                    {
                        t_index_check_status obj = new t_index_check_status();
                        obj.check_flag = sbyte.Parse(indexContents[i].CheckStatus);
                        obj.plan_id = indexContents[i].PlanID;
                        obj.index_id = indexContents[i].ID;
                        db.t_index_check_status.Add(obj);
                    }
                }
                db.SaveChanges();
                return new { code = 200, message = "OK" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        /// <summary>
        /// 等级转换
        /// </summary>
        /// <param name="result"></param>            
        /// <returns></returns>
        public object GetEndCommment(pf_datastatisticContext db, string result, string div)
        {
            try
            {
                string strResult = "H";
                switch (result)
                {
                    case "111":
                        strResult = "A";
                        break;
                    case "110":
                        strResult = "B";
                        break;
                    case "100":
                        strResult = "C";
                        break;
                    case "101":
                        strResult = "D";
                        break;
                    case "011":
                        strResult = "E";
                        break;
                    case "010":
                        strResult = "F";
                        break;
                    case "001":
                        strResult = "G";
                        break;
                    case "000":
                        strResult = "H";
                        break;
                }
                var queryEnd = db.t_end_comment.Where(x => x.delete_flag == 0 && x.end_level == strResult && x.div == div).FirstOrDefault();
                return new { code = 200, result = new { endComment = queryEnd.end_comment, Suggestion = queryEnd.suggestion }, message = " OK" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

    }
    public class IndexContent
    {
        public long ID { get; set; }
        public string IndexKind { get; set; }
        public string IndexDif { get; set; }
        public string CheckStatus { get; set; }
        public long PlanID { get; set; }
    }

}
