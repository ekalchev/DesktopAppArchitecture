using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ApplicationPoc
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class WpfApp : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var app = new MailApplication();
            app.ShutdownCompleted += App_ShutdownCompleted;
            app.Startup();
            Dispatcher.Invoke(() => app.Run());
        }

        private void App_ShutdownCompleted(object sender, EventArgs e)
        {
            Shutdown();
        }
    }
}
