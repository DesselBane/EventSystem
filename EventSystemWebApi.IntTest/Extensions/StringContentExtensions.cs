using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace EventSystemWebApi.IntTest.Extensions
{
    public static class StringContentExtensions
    {
        public static StringContent ToStringContent(this object data)
        {
            return new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        }
    }
}