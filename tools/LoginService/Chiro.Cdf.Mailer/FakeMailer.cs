using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Cdf.Mailer
{
    public class FakeMailer: IMailer
    {
        public bool Verzenden(string ontvanger, string onderwerp, string body)
        {
            return true;
        }
    }
}
