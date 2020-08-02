using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataStatistic.BLL
{
    public class Crud
    {
        public object Get()
        {
            using (var db = new pf_datastatisticContext())
            {
                //var query = from t in db.t_statistic_studata
                //            select t;
                var query = db.t_statistic_studata.ToList();
                return query.ToList();
            }
        }
        public object Update()
        {
            using (var db = new pf_datastatisticContext())
            {
                t_statistic_studata studata = new t_statistic_studata();
                studata.id = 1;
                studata.learning_time = 77;
                db.Entry(studata).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                // db.SaveChanges();

                //db.Set<t_statistic_studata>().Update(studata);
                db.t_statistic_studata.Update(studata);
                db.SaveChanges();
                //db.Database.BeginTransaction();
                //db.Database.CommitTransaction();
            }
            return null;
        }

        public object Delete()
        {
            using (var db = new pf_datastatisticContext())
            {
                t_statistic_studata studata = new t_statistic_studata();
                studata.id = 1;
                db.t_statistic_studata.Remove(studata);
                db.SaveChanges();
            }
            return null;
        }

        public object Create()
        {
            IDbContextTransaction tx = null;


                using (var db = new pf_datastatisticContext())
                using (tx = db.Database.BeginTransaction())
                {
                    try
                    {
                        t_index_check_status obj1 = new t_index_check_status();
                        obj1.index_id = 10;
                        db.t_index_check_status.Add(obj1);
                        db.SaveChanges();

                        t_index_check_status obj2 = new t_index_check_status();
                        obj2.id = 100;
                        obj2.index_id = 11;
                        db.t_index_check_status.Add(obj2);
                        db.SaveChanges();

                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                        PubMethod.ErrorLog(ex, path);
                        tx.Rollback();
                        return new { code = 400, msg = "Error" };
                    }

                }
                return new { code = 200 };

        }

        public object Create1()
        {
            IDbContextTransaction tx = null;

            using (var db = new pf_datastatisticContext())
            {
                try
                {
                    t_index_check_status obj1 = new t_index_check_status();
                    obj1.index_id = 10;
                    db.t_index_check_status.Add(obj1);
                    db.SaveChanges();

                    t_index_check_status obj2 = new t_index_check_status();
                    obj2.id = 100;
                    obj2.index_id = 11;
                    db.t_index_check_status.Add(obj2);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                    PubMethod.ErrorLog(ex, path);
                    return new { code = 400, msg = "Error" };
                }

            }
            return new { code = 200 };

        }
    }
}
