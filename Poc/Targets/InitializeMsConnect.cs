using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationPoc.Targets
{
    class InitializeMsConnect : ITarget
    {
        public string Name => "InitializeMsConnect";

        public bool CanExecute(StartupContext context) => true;

        public void Execute(StartupContext context)
        {
            context.CreateTask<int>("MsConnect connected");
            context.CreateTask<int>("MsConnect Authenticated");

            ExecuteAsync(context);
        }

        private async Task ExecuteAsync(StartupContext context)
        {
            var connectTask = Task.Run(() => { Thread.Sleep(5000); return 1; });
            context.ResolveTask("MsConnect connected", connectTask);
            await connectTask;

            var authenticateTask = Task.Run(() => { Thread.Sleep(1000); return 2; });
            context.ResolveTask("MsConnect Authenticated", authenticateTask);
        }
    }
}
