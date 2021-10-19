using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationPoc.Targets
{
    internal class StartupTarget : ITarget
    {
        public string Name => "Startup";

        public bool CanExecute(StartupContext context) => true;

        public void Execute(StartupContext context)
        {
        }
    }
}
