using GalaSoft.MvvmLight;

namespace CoursewareModifyTool.Model
{
    public class ConfigFile : ObservableObject
    {
        private string _dbAddress;

        public string DbAddress
        {
            get { return _dbAddress;}
            set
            {
                _dbAddress = value;
                RaisePropertyChanged(()=>DbAddress);
            }
        }

        private string _import;
        public string Import
        {
            get { return _import;}
            set
            {
                _import = value;
                RaisePropertyChanged(()=>Import);
            }
        }

        private string _coursePath;

        public string CoursePath
        {
            get { return _coursePath; }
            set
            {
                _coursePath = value;
                RaisePropertyChanged(()=>CoursePath);
            }
        }
    }
}
