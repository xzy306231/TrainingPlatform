using Microsoft.Extensions.Configuration;
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

    Task<string> GetRemoteJson(string str);
    Task<bool> PostRequest(string url, object obj);
    Task<bool> PutRequest(string url, object obj);
}
public class HttpClientHelper : IHttpClientHelper
{
    private DiscoveryHttpClientHandler _handler;
    private IConfiguration Configuration { get; }
    /// <summary>
    /// 客户端发现
    /// </summary>
    /// <param name="client"></param>
    public HttpClientHelper(IDiscoveryClient client, IConfiguration configuration)
    {
        _handler = new DiscoveryHttpClientHandler(client);
        Configuration = configuration;
    }

    /// <summary>
    /// 获取Token中的Json数据
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public async Task<string> GetTokenJson(string str)
    {
        string strResult = Configuration["TokenUrl"]+str;
        var client = new HttpClient(_handler, false);
        return await client.GetStringAsync(strResult);
    }

    public async Task<string> GetRemoteJson(string str)
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

