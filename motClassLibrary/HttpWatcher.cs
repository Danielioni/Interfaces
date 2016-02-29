using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Serialization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;


/// <summary>
/// 
/// Note:  Nothing here attually works
///
/// 
/// </summary>

namespace motInboundLib
{
    public class jsonData
        {
            public jsonData()
            {
            }
        }

    public class HttpWatcher : inputMethod
    {

        public async void watchJsonSite(string url, string node, jsonData input)
        {
            //
            // Do a timed GET against stuff, with "stuff" TBD
            //
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(node);
                if (response.IsSuccessStatusCode)
                {
                    string stuff = await response.Content.ReadAsStringAsync();
                    //
                    // This should be a jsonObject or something
                    //
                }
            }
        }

        public HttpWatcher()
        {
            jsonData j = new jsonData();
            watchJsonSite("http://fred.medicineontime.com/test", "add/thing/1", j);
        }

        public HttpWatcher(string url)
        {
            jsonData j = new jsonData();
            watchJsonSite(url, "add/thing/1", j);
        }
    }
}