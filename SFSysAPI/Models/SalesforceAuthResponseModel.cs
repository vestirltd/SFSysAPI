using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFSysAPI.Models
{
    public class SalesforceAuthResponseModel
    {
        public string access_token { get; set; }
        public string instance_url { get; set; }
        public string id { get; set; }
        public string token_type { get; set; }
        public string issued_at { get; set; }
        public string signature { get; set; }
    }
}

/*
{
    "access_token": "00D4L000000hekI!ARoAQOCngPgnRHMHg.uWhSujuwQgKajDAEHZeP9bupKkblKesAfAbGeXSSXmt3ZANOC0ayD7wExxtkWH2Xo6DKDy0SGOyvDs",
    "instance_url": "https://stsl-dev-ed.my.salesforce.com",
    "id": "https://login.salesforce.com/id/00D4L000000hekIUAQ/0054L000001zwkCQAQ",
    "token_type": "Bearer",
    "issued_at": "1716028309041",
    "signature": "ZhxDqzBwPFDaE1Kw0w9Gf2SVR1CYydhQG275f2Y4qX0="
}
*/