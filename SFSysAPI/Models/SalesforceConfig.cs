using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFSysAPI.Models
{
    public class SalesforceConfig
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SecurityToken { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string tokenUrl { get; set; }
        public string serviceUrl { get; set; }
        public string basePath { get; set; }
        public string queryPath { get; set; }
        public string enKey { get; set; }

    }
}