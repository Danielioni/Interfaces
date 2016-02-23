using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Serialization;


namespace motInboundLib
{
    public class HttpWatcher : inputMethod
    {
        public void watchSite(string url)
        {
            //
            // Do a timed GET against stuff, with "stuff" TBD
            //
        }

        public HttpWatcher()
        {
            watchSite("http://fred.medicineontime.com/test");
        }

        public HttpWatcher(string url)
        {
            watchSite(url);
        }
    }
}