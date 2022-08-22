using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EZTracAdminRSC.Models
{
    public class AppSettings
    {
        public string authorizationEndpoint { get; set; }

        public string callbackURL { get; set; }

        public string scope { get; set; }

        public string responseType { get; set; }

        public string Authority { get; set; }

        public string clientID { get; set; }

        public string clientSecret { get; set; }

        public string jwks_uri { get; set; }
    }
}
