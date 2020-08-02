using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CoursewareDev.BLL
{
    public class RemoteService
    {
        public object GetPageScript(pf_courseware_devContext db, long id)
        {
            List<CoursewarePage> list = new List<CoursewarePage>();
            var query = db.t_courseware_page_bus.Where(x => x.courseware_resource_id == id && x.delete_flag == 0).ToList();
            foreach (var item in query)
            {
                list.Add(new CoursewarePage
                {
                    PageScript = item.page_script,
                    Sort = (int)item.page_sort
                });
            }
            return list;
        }
    }
}
