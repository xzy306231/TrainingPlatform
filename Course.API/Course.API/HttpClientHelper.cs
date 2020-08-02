using Course.BLL;
using Course.DAL;
using Steeltoe.Common.Discovery;
using System.Net.Http;
using System.Threading.Tasks;

namespace Course.API
{
    /// <summary>
    /// 接口
    /// </summary>
    public interface IHttpClientHelper
    {
        /// <summary>
        /// 获取Token中的Json数据
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        Task<string> GetTokenJson(string str);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        Task<string> GetRemoteJson(string str);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        Task<bool> PutRequest(string url, object obj);
    }


    /// <summary>
    /// 接口实现
    /// </summary>
    public class HttpClientHelper : IHttpClientHelper
    {
        private DiscoveryHttpClientHandler _handler;

        /// <summary>
        /// 客户端发现
        /// </summary>
        /// <param name="client"></param>
        public HttpClientHelper(IDiscoveryClient client)
        {
            _handler = new DiscoveryHttpClientHandler(client);
        }

        /// <summary>
        /// 获取Token中的Json数据
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public async Task<string> GetTokenJson(string str)
        {
            var client = new HttpClient(_handler, false);
            string strResult = PubMethod.ReadConfigJsonData("TokenUrl")+str;
            return await client.GetStringAsync(strResult);
        }

        public async Task<string> GetRemoteJson(string str)
        {
            var client = new HttpClient(_handler, false);
            return await client.GetStringAsync(str);
        }

        public async Task<bool> PutRequest(string url,object obj)
        {
            var client = new HttpClient(_handler, false);
            return await client.PutAsJsonAsync(url, obj).ContinueWith(x => x.Result.IsSuccessStatusCode);
        }
    }
}
