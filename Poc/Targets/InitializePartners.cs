using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationPoc.Targets
{
    class InitializePartners : ITarget
    {
        public string Name => "InitializePartners";

        public bool CanExecute(StartupContext context) => true;

        public void Execute(StartupContext context)
        {
            // initialize partners code
        }
    }
}
