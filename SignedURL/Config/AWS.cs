using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignedURL.Config
{
    public class AWS
    {
        public AWS() { }
        public string region { get; set; }
        public string accessKey { get; set; }
        public string secretKey { get; set; }
    }
}

