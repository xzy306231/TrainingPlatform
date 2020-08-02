using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoursewareDev.BLL
{
    public class Courseware
    {
        public object GetNotPublishCourseware(pf_courseware_devContext db, string keyWord, int pageIndex = 1, int pageSize = 10)
        {

            try
            {
                var query = db.t_courseware.Where(x => x.delete_flag == 0 && (string.IsNullOrEmpty(keyWord) ? true : x.courseware_title.Contains(keyWord)) && x.publish_flag == 0);
                var queryList = query.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                List<CoursewareInfo> list = new List<CoursewareInfo>();
                foreach (var item in queryList)
                {
                    list.Add(new CoursewareInfo
                    {
                        id = item.id,
                        CoursewareTitle = item.courseware_title,
                        CreateName = item.create_name,
                        UpdateTime = item.t_modified.ToString()
                    });
                }
                return new { code = 200, result = new { list, count = query.Count() }, message = "OK" };
            }
            catch (Exception ex)
            {
                return new { code = 400, message = "Error" };
            }
        }
        public object GetPublishedCourseware(pf_courseware_devContext db, string keyWord, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var query = db.t_courseware.Where(x => x.delete_flag == 0 && (string.IsNullOrEmpty(keyWord) ? true : x.courseware_title.Contains(keyWord)) && x.publish_flag == 1);
                var queryList = query.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                List<CoursewareInfo> list = new List<CoursewareInfo>();
                foreach (var item in queryList)
                {
                    list.Add(new CoursewareInfo
                    {
                        id = item.id,
                        CoursewareTitle = item.courseware_title,
                        CreateName = item.create_name,
                        UpdateTime = item.t_modified.ToString()
                    });
                }
                return new { code = 200, result = new { list, count = query.Count() }, message = "OK" };
            }
            catch (Exception)
            {
                return new { code = 400, message = "Error" };
            }
        }
        public async Task<object> PublishCourseware(pf_courseware_devContext db, List<long> idList)
        {
            try
            {
                bool publishFlag = true;
                List<CoursewareInfo> list = new List<CoursewareInfo>();
                var query = db.t_courseware.Where(x => x.delete_flag == 0 && idList.Contains(x.id)).ToList();
                foreach (var item in query)
                {
                    item.publish_flag = 1;
                    CoursewareInfo courseware = new CoursewareInfo();
                    courseware.CoursewareTitle = item.courseware_title;
                    courseware.CreateID = item.create_id;
                    courseware.CreateName = item.create_name;
                    courseware.CreateNumber = item.create_number;
                    courseware.fileSize = item.file_size;

                    List<CoursewarePage> pageList = new List<CoursewarePage>();
                    var queryScript = db.t_courseware_page.Where(x => x.delete_flag == 0 && x.courseware_id == item.id).OrderBy(x => x.page_sort).ToList();
                    if (queryScript.Count == 0)
                    {
                        publishFlag = false;
                        break;
                    }
                    foreach (var item1 in queryScript)
                    {
                        pageList.Add(new CoursewarePage
                        {
                            PageScript = item1.page_script,
                            Sort = (int)item1.page_sort
                        });
                        courseware.PageList = pageList;
                    }
                    list.Add(courseware);
                }
                if (publishFlag == false)
                    return new {code=400,message="部分课件不存在内容！" };
                AddCoursewarePage(db, list);
                await db.SaveChangesAsync();
                return new { code = 200, message = "OK" };
            }
            catch (Exception)
            {
                return new { code = 400, message = "Error" };
            }
        }
        private void AddCoursewarePage(pf_courseware_devContext db, List<CoursewareInfo> list)
        {
            try
            {
                if (list != null && list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        t_course_resource resource = new t_course_resource();
                        resource.resource_name = list[i].CoursewareTitle;
                        resource.resource_type = "6";
                        resource.creator_id = list[i].CreateID;
                        resource.creator_name = list[i].CreateName;
                        resource.file_suffix = "px";
                        resource.check_status = "2";
                        resource.file_size = list[i].fileSize;
                        resource.file_size_display = list[i].fileSize.ToString() + "KB";
                        resource.transf_type = "0";
                        resource.resource_duration = 0;
                        resource.checker_id = 0;
                        db.Add(resource);
                        db.SaveChanges();
                        long id = resource.id;
                        if (list[i].PageList != null && list[i].PageList.Count > 0)
                        {
                            for (int j = 0; j < list[i].PageList.Count; j++)
                            {
                                t_courseware_page_bus pageBus = new t_courseware_page_bus();
                                pageBus.courseware_resource_id = id;
                                pageBus.page_script = list[i].PageList[j].PageScript;
                                pageBus.page_sort = list[i].PageList[j].Sort;
                                db.t_courseware_page_bus.Add(pageBus);
                            }
                        }
                    }
                }
                db.SaveChanges();

            }
            catch (Exception)
            {

            }
        }
        public async Task<object> RemoveCourseware(pf_courseware_devContext db, List<long> idList)
        {
            try
            {
                var query = db.t_courseware.Where(x => x.delete_flag == 0 && idList.Contains(x.id)).ToList();
                foreach (var item in query)
                {
                    item.delete_flag = 1;
                }
                await db.SaveChangesAsync();
                return new { code = 200, message = "OK" };
            }
            catch (Exception)
            {
                return new { code = 400, message = "Error" };
            }
        }
        public async Task<object> CreateCourseware(pf_courseware_devContext db, CoursewareInfo courseware)
        {
            try
            {
                t_courseware ware = new t_courseware();
                ware.courseware_title = courseware.CoursewareTitle;
                ware.resource_confidential = courseware.ResourceConfidential;
                ware.update_number = courseware.CreateNumber;
                ware.create_name = courseware.CreateName;
                ware.create_number = courseware.CreateNumber;
                ware.create_id = courseware.CreateID;
                db.Add(ware);
                await db.SaveChangesAsync();
                return new { code = 200, result = ware.id, message = "OK" };
            }
            catch (Exception)
            {
                return new { code = 400, message = "Error" };
            }
        }
        public object GetPageContent(pf_courseware_devContext db, long id)
        {
            try
            {
                CoursewareInfo courseware = new CoursewareInfo();
                var queryCourseware = db.t_courseware.Find(id);
                courseware.CoursewareTitle = queryCourseware.courseware_title;
                var query = db.t_courseware_page.Where(x => x.delete_flag == 0 && x.courseware_id == id).OrderBy(x => x.page_sort).ToList();
                List<CoursewarePage> list = new List<CoursewarePage>();
                foreach (var item in query)
                {
                    list.Add(new CoursewarePage
                    {
                        CoursewareId = item.id,
                        PageScript = item.page_script,
                        Sort = (int)item.page_sort
                    });
                }
                courseware.PageList = list;
                return new { code = 200, result = courseware, message = "OK" };
            }
            catch (Exception)
            {
                return new { code = 400, message = "Error" };
            }
        }
        public object PreviewPageContent(pf_courseware_devContext db, long id)
        {
            try
            {
                CoursewareInfo courseware = new CoursewareInfo();
                var queryCourseware = db.t_course_resource.Find(id);
                courseware.CoursewareTitle = queryCourseware.resource_name;
                var query = db.t_courseware_page_bus.Where(x => x.delete_flag == 0 && x.courseware_resource_id == id).OrderBy(x => x.page_sort).ToList();
                List<CoursewarePage> list = new List<CoursewarePage>();
                foreach (var item in query)
                {
                    list.Add(new CoursewarePage
                    {
                        CoursewareId = item.id,
                        PageScript = item.page_script,
                        Sort = (int)item.page_sort
                    });
                }
                courseware.PageList = list;
                return new { code = 200, result = courseware, message = "OK" };
            }
            catch (Exception ex)
            {
                return new { code = 400, message = "Error" };

            }
        }
        public async Task<object> SaveCourseware(pf_courseware_devContext db, CoursewareInfo coursewareInfo)
        {
            try
            {
                var queryCourseware = db.t_courseware.Find(coursewareInfo.id);
                queryCourseware.courseware_title = coursewareInfo.CoursewareTitle;
                queryCourseware.t_modified = DateTime.Now;
                if (coursewareInfo.fileSize == 0)
                    queryCourseware.file_size = 1;
                else
                {
                    int n = coursewareInfo.fileSize / 1024;
                    if (n == 0)
                        queryCourseware.file_size = 1;
                    else
                        queryCourseware.file_size = n;
                }
                var query = db.t_courseware_page.Where(x => x.delete_flag == 0 && x.courseware_id == coursewareInfo.id).ToList();
                foreach (var item in query)
                {
                    item.delete_flag = 1;
                }
                if (coursewareInfo.PageList != null && coursewareInfo.PageList.Count > 0)
                {
                    int sort = 0;
                    for (int i = 0; i < coursewareInfo.PageList.Count; i++)
                    {
                        t_courseware_page page = new t_courseware_page();
                        page.page_sort = ++sort;
                        page.courseware_id = coursewareInfo.id;
                        page.page_script = coursewareInfo.PageList[i].PageScript;
                        db.Add(page);
                    }
                }
                await db.SaveChangesAsync();
                return new { code = 200, message = "OK" };
            }
            catch (Exception)
            {
                return new { code = 400, message = "Error" };
            }
        }
    }
    public class CoursewareInfo
    {
        public long id { get; set; }
        public string CoursewareTitle { get; set; }
        public string ResourceConfidential { get; set; }
        public string UpdateTime { get; set; }
        public string CreateName { get; set; }
        public int fileSize { get; set; }
        public long CreateID { get; set; }
        public string CreateNumber { get; set; }
        public List<CoursewarePage> PageList { get; set; }
    }
    public class CoursewarePage
    {
        public long CoursewareId { get; set; }
        public int Sort { get; set; }
        public string PageScript { get; set; }
    }
}
