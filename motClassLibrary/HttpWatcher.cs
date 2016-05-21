// 
// MIT license
//
// Copyright (c) 2016 by Peter H. Jenney and Medicine-On-Time, LLC.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 

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