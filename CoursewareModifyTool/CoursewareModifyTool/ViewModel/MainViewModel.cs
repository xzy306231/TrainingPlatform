using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoursewareModifyTool.Helper;
using CoursewareModifyTool.Model;
using CoursewareModifyTool.Model.Courseware;
using CoursewareModifyTool.View;
using FreeSql;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MaterialDesignThemes.Wpf;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using NLog;

namespace CoursewareModifyTool.ViewModel
{

    public class MainViewModel : ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        private ConfigFile _configFile;

        public ConfigFile ConfigInfo
        {
            get { return _configFile; }
            set
            {
                _configFile = value;
                RaisePropertyChanged(()=>ConfigInfo);
            }
        }

        private readonly string[] _tempStrArr = new string[1];


        private string _promptBoxInfo;
        public string PromptBoxInfo
        {
            get => _promptBoxInfo;
            set
            {
                _promptBoxInfo = value;
                RaisePropertyChanged(() => PromptBoxInfo);
            }
        }

        private string _swfFolder;

        public string SwfFolder
        {
            get => _swfFolder;
            set
            {
                _swfFolder = value;
                RaisePropertyChanged(()=>SwfFolder);
            }
        }


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            var currentPath = Environment.CurrentDirectory;
            ConfigInfo = JsonConvert.DeserializeObject<ConfigFile>(File.ReadAllText(currentPath + "\\Configuration.json"));
            InitOrm();
            OriginalSwfList = new ObservableCollection<string>();
            NewSwfList = new ObservableCollection<string>();
            AddSwfList = new ObservableCollection<string>();

            _tempStrArr[0] = ":/";

            SwfFolder = ConfigInfo.CoursePath;

        }

        private void InitOrm()
        {
        }

        /// <summary>
        /// 弹出对话框选择文件
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="filterName"></param>
        /// <param name="initialDirectory"></param>
        /// <returns></returns>
        private List<string> GetFileList(string filter, string filterName, string initialDirectory)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = $"选择{filterName}文件",
                Filter = $"swf文件(*.{filter})|*.{filter}",
                FileName = "",
                FilterIndex = 1,
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                Multiselect = true,
                InitialDirectory = initialDirectory
            };
            var result = openFileDialog.ShowDialog();
            return result != true ? null : openFileDialog.FileNames.ToList();
        }

        private bool _dialogShowing;
        /// <summary>
        /// 消息对话框
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task ShowMessage(string message)
        {
            var view = new SampleDialogHint();
            PromptBoxInfo = message;
            _dialogShowing = true;
            await DialogHost.Show(view, "MainDialog");
            PromptBoxInfo = string.Empty;
            _dialogShowing = false;
        }

        /// <summary>
        /// 获取存储目录地址
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string GetUrlWithoutDrive(string path)
        {
            path = path.Replace('\\', '/');
            var temp = path.Split(_tempStrArr, StringSplitOptions.None);
            //var temp = path.Split(":/");
            if (temp.Length == 2) path = temp[1].Substring(4);
            return path;
        }

        /// <summary>
        /// 获取文件字幕
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private async Task<string> GetSubTitle(string filePath)
        {
            var path = filePath.Replace("swf", "txt");
            try
            {
                using (var str = new StreamReader(path,EncodingType.GetType(filePath)))
                {
                    return str.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                await ShowMessage(e.Message);
                return "";
            }
        }

        /// <summary>
        /// 文件大小展示
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private string GetFileSizeDisplay(long size)
        {
            var k = size / 1024;
            if (k <= 1024) return $"{Convert.ToInt32(k)}KB";
            var m = k / 1024;
            if (m < 1024) return $"{Convert.ToInt32(m)}MB";
            var g = k / 1024;
            return $"{g:0.0}GB";
        }

        #region ::::: 修改课件+字幕 :::::

        /// <summary>
        /// 原始课件列表
        /// </summary>
        public ObservableCollection<string> OriginalSwfList { get; set; }

        private RelayCommand _openOriginalSwf;
        /// <summary>
        /// 添加原始课件
        /// </summary>
        public RelayCommand OpenOriginalSwf
        {
            get
            {
                return _openOriginalSwf
                       ?? (_openOriginalSwf = new RelayCommand(
                           () =>
                           {
                               var result = GetFileList("swf", "swf", ConfigInfo.CoursePath);
                               if (result == null) return;
                               foreach (var temp in result)
                               {
                                   OriginalSwfList.Add(temp);
                               }
                           }));
            }
        }

        /// <summary>
        /// 修改后的课件列表
        /// </summary>
        public ObservableCollection<string> NewSwfList { get; set; }

        private RelayCommand _openNewSwf;
        /// <summary>
        /// 添加新课件
        /// </summary>
        public RelayCommand OpenNewSwf
        {
            get
            {
                return _openNewSwf
                       ?? (_openNewSwf = new RelayCommand(
                           () =>
                           {
                               var result = GetFileList("swf", "swf", AppDomain.CurrentDomain.BaseDirectory);
                               if (result == null) return;
                               foreach (var temp in result)
                               {
                                   NewSwfList.Add(temp);
                               }
                           }));
            }
        }

        private RelayCommand _clearSwfList;
        /// <summary>
        /// 清空原始课件列表
        /// </summary>
        public RelayCommand ClearSwfList
        {
            get
            {
                return _clearSwfList
                       ?? (_clearSwfList = new RelayCommand(
                           () =>
                           {
                               OriginalSwfList.Clear();
                               NewSwfList.Clear();
                           }));
            }
        }

        private RelayCommand _updateSwfList;
        /// <summary>
        /// 确认swf更新
        /// </summary>
        public RelayCommand UpdateSwfList
        {
            get
            {
                return _updateSwfList
                       ?? (_updateSwfList = new RelayCommand(
                           async () =>
                           {
                               //todo：修改数据库
                               if (OriginalSwfList.Count == 0 || NewSwfList.Count == 0)
                               {
                                   await ShowMessage("列表不能为空");
                                   return;
                               }

                               if (OriginalSwfList.Count != NewSwfList.Count)
                               {
                                   await ShowMessage("原始课件列表与修改课件列表数量不相等");
                                   return;
                               }

                               for (int index = 0; index < OriginalSwfList.Count; index++)
                               {
                                   if (new FileInfo(OriginalSwfList[index]).Name.Equals(new FileInfo(NewSwfList[index]).Name)) continue;
                                   await ShowMessage($"原始课件列表第{index + 1}条数据与修改课件列表第{index + 1}条数据名称不一样，请修改");
                                   return;
                               }

                               var loadingDialog = new SampleProgressDialog();
                               await DialogHost.Show(loadingDialog, "MainDialog", UpdateSwfEventHandler);

                               OriginalSwfList.Clear();
                               NewSwfList.Clear();
                           }));
            }
        }

        private async void UpdateSwfEventHandler(object sender, DialogOpenedEventArgs eventargs)
        {
            await Task.Run(async () =>
            {
                for (int i = 0; i < OriginalSwfList.Count; i++)
                {
                    File.Copy(NewSwfList[i], OriginalSwfList[i], true);
                    var index = i;
                    var temp = await MainWindow.FSqlCourseware.Select<TCourseResource>()
                        .Where(e => e.TransformUrl.Equals(GetUrlWithoutDrive(OriginalSwfList[index])))
                        .ToOneAsync();
                    if (temp == null)
                    {
                        await ShowMessage($"课件{OriginalSwfList[index]}在数据库不存在");
                        continue;
                    }

                    var subTitle = await GetSubTitle(NewSwfList[i]);
                    if (string.IsNullOrEmpty(subTitle)) continue;
                    {
                        await MainWindow.FSqlCourseware.Update<TCourseResource>(temp.Id)
                            .Set(a => a.ThumbnailPath, subTitle)
                            .ExecuteAffrowsAsync();
                        Logger.Info($"课件资源库课件表更新id为{temp.Id}的字幕");
                        await MainWindow.FSqlCourse.Update<Model.Course.TCourseResource>()
                            .Where(e => e.SrcId == temp.Id)
                            .Set(a => a.ResourceDesc, subTitle)
                            .ExecuteAffrowsAsync();
                        Logger.Info($"课程资源库课件表更新SrcId为{temp.Id}的字幕");
                    }
                }
            }).ContinueWith((t, _) => eventargs.Session.Close(false), null,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion

        #region ::::: 新增课件 :::::
        private RelayCommand _selectFolder;

        public RelayCommand SelectFolder
        {
            get { return _selectFolder 
                         ?? (_selectFolder = new RelayCommand(
                             () =>
                             {
                                 var dialog = new CommonOpenFileDialog
                                 {
                                     IsFolderPicker = true, 
                                     Multiselect = false,
                                     InitialDirectory = ConfigInfo.CoursePath
                                 };
                                 var result = dialog.ShowDialog();
                                 if(result != CommonFileDialogResult.Ok) return;
                                 SwfFolder = dialog.FileNames.FirstOrDefault();
                             })); }
        }


        public ObservableCollection<string> AddSwfList { get; set; }

        private RelayCommand _addNewSwf;
        /// <summary>
        /// 添加新课件
        /// </summary>
        public RelayCommand AddNewSwf
        {
            get
            {
                return _addNewSwf
                       ?? (_addNewSwf = new RelayCommand(
                           () =>
                           {
                               var result = GetFileList("swf", "swf", AppDomain.CurrentDomain.BaseDirectory);
                               if (result == null) return;
                               foreach (var temp in result)
                               {
                                   AddSwfList.Add(temp);
                               }
                           }));
            }
        }

        private RelayCommand _clearNewSwfList;

        /// <summary>
        /// 清空新课件列表
        /// </summary>
        public RelayCommand ClearNewSwfList
        {
            get
            {
                return _clearNewSwfList
                       ?? (_clearNewSwfList = new RelayCommand(
                           () =>
                           {
                               AddSwfList.Clear();
                           }));
            }
        }

        private RelayCommand _addSwfListCommand;
        /// <summary>
        /// 数据库添加记录.
        /// </summary>
        public RelayCommand AddSwfListCommand
        {
            get
            {
                return _addSwfListCommand
                       ?? (_addSwfListCommand = new RelayCommand(
                           async () =>
                           {
                               //todo：新增数据
                               if (AddSwfList.Count == 0)
                               {
                                   await ShowMessage("列表不能为空");
                                   return;
                               }

                               var loadingDialog = new SampleProgressDialog();
                               await DialogHost.Show(loadingDialog, "MainDialog", AddSwfEventHandler);
                               AddSwfList.Clear();
                           }));
            }
        }

        private async void AddSwfEventHandler(object sender, DialogOpenedEventArgs eventargs)
        {
            await Task.Run(async () =>
            {
                foreach (var swfFile in AddSwfList)
                {
                    var tempInfo = new FileInfo(swfFile);
                    var tempFullPath = SwfFolder + "\\" + tempInfo.Name;

                    //如果添加的课件在已有的文件夹中，即知识点已经存在
                    var tempFileList = new DirectoryInfo(SwfFolder).GetFiles();
                    var tempTagList = new List<long>();
                    if (tempFileList.Length != 0)
                    {
                        var tempResource = await MainWindow.FSqlCourseware.Select<TCourseResource>()
                            .Where(e => e.TransformUrl.Equals(GetUrlWithoutDrive(tempFileList[0].FullName)))
                            .IncludeMany(e => e.t_resource_tag_refs, then => then.Include(t => t.t_knowledge_tag))
                            .ToOneAsync();
                        if (tempResource == null)
                        {
                            Logger.Error($"文件{tempFileList[0].FullName}在数据库中不存在对应的记录");
                        }
                        else
                        {
                            tempTagList.AddRange(tempResource.t_resource_tag_refs.Select(tagRef => tagRef.TagId));
                        }
                    }

                    File.Copy(swfFile, tempFullPath, true);
                    var subTitle = await GetSubTitle(swfFile);
                    var id = await MainWindow.FSqlCourseware.Insert<TCourseResource>()
                        .AppendData(new TCourseResource
                        {
                            CheckStatus = "3",
                            CreatorId = MainWindow.Teacher.OriginalId,
                            CreatorName = MainWindow.Teacher.UserName,
                            FileSuffix = "swf",
                            FileSize = tempInfo.Length,
                            FileSizeDisplay = GetFileSizeDisplay(tempInfo.Length),
                            ThumbnailPath = subTitle,
                            TransfType = "0",
                            TransformUrl = GetUrlWithoutDrive(tempFullPath),
                            ResourceLevel = "3",
                            ResourceName = tempInfo.Name,
                            ResourceType = "2",
                            TCreate = DateTime.Now
                        }).ExecuteIdentityAsync();
                    Logger.Info($"课件资源库课件表新增id为{id}的课件");
                    if (tempTagList.Count == 0) continue;
                    foreach (var tagId in tempTagList)
                    {
                        await MainWindow.FSqlCourseware.Insert<TResourceTagRef>()
                            .AppendData(new TResourceTagRef
                            {
                                DeleteFlag = 0, ResourceId = id, TagId = tagId, TCreate = DateTime.Now
                            }).ExecuteAffrowsAsync();
                        Logger.Info($"课件资源库课件知识点关联表新增课件id为{id},知识点id为{tagId}的数据");
                    }
                }
            }).ContinueWith((t, _) => eventargs.Session.Close(false), null,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion


        private RelayCommand _saveConfig;
        /// <summary>
        /// 清空原始课件列表
        /// </summary>
        public RelayCommand SaveConfig
        {
            get
            {
                return _saveConfig
                       ?? (_saveConfig = new RelayCommand(
                           async () =>
                           {
                               var loadingDialog = new SampleProgressDialog();
                               await DialogHost.Show(loadingDialog, "MainDialog", SaveConfigFileEventHandler);
                               await ShowMessage("保存完毕,请重启程序加载新配置");
                           }));
            }
        }

        private async void SaveConfigFileEventHandler(object sender, DialogOpenedEventArgs eventargs)
        {
            await Task.Run(async () =>
            {
                using (var sw = new StreamWriter(Environment.CurrentDirectory + "\\Configuration.json", false))
                {
                    await sw.WriteAsync(JsonConvert.SerializeObject(ConfigInfo));
                }
            }).ContinueWith((t, _) => eventargs.Session.Close(false), null,
                TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}