using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationPoc.Targets
{
    class InitializeSentry : ITarget
    {
        public string Name => "InitializeSentry";

        public bool CanExecute(StartupContext context) => true;

        public void Execute(StartupContext context)
        {
            // initialize sentry code
        }
    }
}
