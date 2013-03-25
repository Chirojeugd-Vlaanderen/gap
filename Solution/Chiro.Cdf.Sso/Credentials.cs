using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Cdf.Sso
{
    /// <summary>
    /// Credentials voor single sign on
    /// </summary>
    public class Credentials
    {
        public string GeencrypteerdeUserInfo { get; set; }
        public string Hash { get; set; }
    }
}
