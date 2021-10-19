using ApplicationPoc.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationPoc
{
    abstract class OfficeSuiteApplication : Application
    {
        protected override void RegisterTargets()
        {
            ExecuteAfter(new RegisterDependencyInjectionTypes(), "Startup");
            ExecuteAfter(new InitializeLogger(), "RegisterDependencyInjectionTypes");
            ExecuteAfter(new InitializeSentry(), "InitializeLogger");
            ExecuteAfter(new InitializeMsConnect(), "InitializeSentry");
        }

        protected abstract void ShowFirstWindow();

        public override void Run()
        {
            ShowFirstWindow();
        }
    }
}
