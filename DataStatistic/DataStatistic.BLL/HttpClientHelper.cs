using Steeltoe.Common.Discovery;
using Steeltoe.Common.Http;
using System.Net.Http;
using System.Threading.Tasks;

public interface IHttpClientHelper
{
    /// <summary>
    /// 获取Token中的Json数据
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    Task<string> GetTokenJson(string str);
    Task<string> GetRequest(string str);
    Task<bool> PostRequest(string url, object obj);
    Task<bool> PutRequest(string url, object obj);
}

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
        string strResult = PubMethod.ReadConfigJsonData("TokenUrl") + str;
        return await GetRequest(strResult);
    }

    public async Task<string> GetRequest(string str)
    {
        var client = new HttpClient(_handler, false);
        return await client.GetStringAsync(str);
    }

    public async Task<bool> PostRequest(string url, object obj)
    {
        var client = new HttpClient(_handler, false);
        return await client.PostAsJsonAsync(url, obj).ContinueWith(x => x.Result.IsSuccessStatusCode);
    }
    public async Task<bool> PutRequest(string url, object obj)
    {
        var client = new HttpClient(_handler, false);
        return await client.PutAsJsonAsync(url, obj).ContinueWith(x => x.Result.IsSuccessStatusCode);
    }
}
