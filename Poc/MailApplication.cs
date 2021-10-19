using ApplicationPoc.Targets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ApplicationPoc
{
    class MailApplication : OfficeSuiteApplication
    {
        private MailClientStartupContext context;
        protected override StartupContext CreateStartupContext()
        {
            context = new MailClientStartupContext();
            return context;
        }

        protected override void ShowFirstWindow()
        {
            // show first window
            MainWindow window = new MainWindow();
            window.Loaded += Window_Loaded;
            window.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ExecutePostLoadActions();
        }

        private async Task ExecutePostLoadActions()
        {
            var result = await context.Task<int>("MsConnect Authenticated");
            Debug.WriteLine("MSConnect was authenticated");
        }

        protected override void RegisterTargets()
        {
            base.RegisterTargets();

            ExecuteAfter(new UwpStoreInitialize(false), "Startup");
            ExecuteBefore(new InitializeSQLiteDatabase(), "InitializeMsConnect");
            ExecuteAfter(new LoadMailAccounts(), "InitializeMsConnect");
        }
    }
}
