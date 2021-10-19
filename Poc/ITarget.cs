using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationPoc
{
    public interface ITarget
    {
        public string Name { get; }
        public void Execute(StartupContext context);
        public bool CanExecute(StartupContext context);
    }
}
