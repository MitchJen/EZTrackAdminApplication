using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EZTracAdminRSC.Models
{
    public class AWS
    {
        public string Profile { get; set; }

        public string AccessKey { get; set; }

        public string SecretKey { get; set; }

        public string Region { get; set; }
    }
}