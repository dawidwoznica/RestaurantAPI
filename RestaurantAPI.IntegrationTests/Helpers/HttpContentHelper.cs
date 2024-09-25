namespace RestaurantAPI.IntegrationTests.Helpers;

using System.Text;
using Newtonsoft.Json;

public static class HttpContentHelper
{
    public static HttpContent ToJsonHttpContent(this object obj)
    {
        var jsonModel = JsonConvert.SerializeObject(obj);
        return new StringContent(jsonModel, Encoding.UTF8, "application/json");
    }
}