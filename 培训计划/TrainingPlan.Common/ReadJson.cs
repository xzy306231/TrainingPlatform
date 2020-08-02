using Microsoft.Extensions.Configuration;
using System;
using System.IO;


    public class ReadJson
    {
        public string ReadJsonData(string strSectionName)
        {
            try
            {
                //添加 json 文件路径
                var builder = new ConfigurationBuilder().SetBasePath(Path.GetDirectoryName(this.GetType().Assembly.Location)).AddJsonFile("dalsettings.json");
                //创建配置根对象
                var configurationRoot = builder.Build();
                //取配置根下的 name 部分
                var nameSection = configurationRoot.GetSection(strSectionName);
                return nameSection.Value;
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) +  @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return ex.Message;
            }
        }
    }

