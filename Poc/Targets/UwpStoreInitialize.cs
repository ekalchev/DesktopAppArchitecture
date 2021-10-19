using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationPoc.Targets
{
    class UwpStoreInitialize : ITarget
    {
        private readonly bool isUwpApp;

        public UwpStoreInitialize(bool isUwpApp)
        {
            this.isUwpApp = isUwpApp;
        }

        public string Name => "UwpStoreInitialize";

        public bool CanExecute(StartupContext context) => isUwpApp;

        public void Execute(StartupContext context)
        {
        }
    }
}
