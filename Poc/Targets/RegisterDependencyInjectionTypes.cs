using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationPoc.Targets
{
    class RegisterDependencyInjectionTypes : ITarget
    {
        public string Name => "RegisterDependencyInjectionTypes";

        public bool CanExecute(StartupContext context) => true;

        public void Execute(StartupContext context)
        {
        }
    }
}
