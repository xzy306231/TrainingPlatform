using Microsoft.Extensions.Configuration;
using Steeltoe.Common.Discovery;
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
        var client = new HttpClient(_handler, false);
        string strResult = Configuration["TokenUrl"]+str;
        return await client.GetStringAsync(strResult);
    }

    public async Task<string> GetRemoteJson(string str)
    {
        var client = new HttpClient(_handler, false);
        return await client.GetStringAsync(str);
    }
}

