using System;
using System.IO;
using System.Windows;
using CoursewareModifyTool.Model;
using FreeSql;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;

namespace CoursewareModifyTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public static IFreeSql FSqlCourseware { get; set; }
        public static IFreeSql FSqlCourse { get; set; }
        public static IFreeSql FSqlPeopleManage { get; set; }

        public static TPersonInfo Teacher;

        private readonly ResourceDictionary _dialogDictionary = new ResourceDictionary
            { Source = new Uri("pack://application:,,,/MaterialDesignThemes.MahApps;component/Themes/MaterialDesignTheme.MahApps.Dialogs.xaml") };

        public MainWindow()
        {
            InitializeComponent();

            var currentPath = Environment.CurrentDirectory;
            var configInfo = JsonConvert.DeserializeObject<ConfigFile>(File.ReadAllText(currentPath + "\\Configuration.json"));

            try
            {
                FSqlCourse = new FreeSqlBuilder().UseConnectionString(DataType.SqlServer,
                        $"Server={configInfo.DbAddress};Initial Catalog=pf_course_manage_v1;Trusted_Connection=False;User ID=sa;Password=admin@123;MultipleActiveResultSets=true")
                    .Build();
                FSqlCourseware = new FreeSqlBuilder().UseConnectionString(DataType.SqlServer,
                        $"Server={configInfo.DbAddress};Initial Catalog=pf_course_resource;Trusted_Connection=False;User ID=sa;Password=admin@123;MultipleActiveResultSets=true")
                    .Build();
                FSqlPeopleManage = new FreeSqlBuilder().UseConnectionString(DataType.SqlServer,
                        $"Server={configInfo.DbAddress};Initial Catalog=pf_people_manage;Trusted_Connection=False;User ID=sa;Password=admin@123;MultipleActiveResultSets=true")
                    .Build();

                Teacher = MainWindow.FSqlPeopleManage.Select<TPersonInfo>().Where(p => p.UserName.Equals(configInfo.Import)).ToOne();
            }
            catch (Exception e)
            {
                var metroDialogSetting = new MetroDialogSettings()
                {
                    CustomResourceDictionary = _dialogDictionary,
                    NegativeButtonText = "OK"
                };
                this.ShowMessageAsync("错误", $"数据库连接失败：{e.Message}", MessageDialogStyle.Affirmative, metroDialogSetting);
            }
        }
    }
}
