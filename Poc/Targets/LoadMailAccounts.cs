using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationPoc.Targets
{
    class LoadMailAccounts : ITarget
    {
        public string Name => "LoadMailAccounts";

        public bool CanExecute(StartupContext context) => true;

        public void Execute(StartupContext context)
        {
            MailClientStartupContext mailClientContext = (MailClientStartupContext)context;

            if(mailClientContext.IsMsConnectInitialized == true)
            {
                //...
            }
        }
    }
}
