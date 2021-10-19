using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationPoc
{
    class MailClientStartupContext : OfficeSuiteStartupContext
    {
        public bool IsSQLiteDatabaseInitialized { get; set; }
    }
}
