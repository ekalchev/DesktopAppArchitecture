using ApplicationPoc.Targets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationPoc
{
    public abstract class Application
    {
        public event EventHandler<EventArgs> ShutdownCompleted;
        private static ITarget startupTarget = new StartupTarget();
        private Dictionary<string, List<ITarget>> executeAfter = new Dictionary<string, List<ITarget>>();
        private Dictionary<string, List<ITarget>> executeBefore = new Dictionary<string, List<ITarget>>();

        public void Startup()
        {
            RegisterTargets();
            List<ITarget> startupSequence = new List<ITarget>();
            BuildStartupSequence(startupTarget, startupSequence);
            LogStartupSequence(startupSequence);

            StartupContext startupContext = CreateStartupContext();
            foreach (var target in startupSequence)
            {
                if(target.CanExecute(startupContext) == true)
                {
                    target.Execute(startupContext);
                }
            }
        }

        public virtual void Run()
        {

        }

        protected abstract StartupContext CreateStartupContext();
        protected abstract void RegisterTargets();

        private void BuildStartupSequence(ITarget currentTarget, List<ITarget> result)
        {
            if(executeBefore.ContainsKey(currentTarget.Name) == true)
            {
                foreach(var beforeTarget in executeBefore[currentTarget.Name])
                {
                    BuildStartupSequence(beforeTarget, result);
                }
            }

            result.Add(currentTarget);

            if (executeAfter.ContainsKey(currentTarget.Name) == true)
            {
                foreach (var afterTarget in executeAfter[currentTarget.Name])
                {
                    BuildStartupSequence(afterTarget, result);
                }
            }
        }

        private void LogStartupSequence(IEnumerable<ITarget> startupSequence)
        {
            int counter = 0;
            foreach(var target in startupSequence)
            {
                Debug.WriteLine($"{++counter}:{target.Name}");
            }
        }

        protected void ExecuteBefore(ITarget target, string targetName)
        {
            if(startupTarget.Name == targetName)
            {
                throw new InvalidOperationException($"You can't execute action before '{targetName}'");
            }

            AddTarget(target, executeBefore, targetName);
        }

        protected void ExecuteAfter(ITarget startupAction, string targetName)
        {
            AddTarget(startupAction, executeAfter, targetName);
        }

        private void AddTarget(ITarget target, Dictionary<string, List<ITarget>> container, string targetName)
        {
            if (container.ContainsKey(targetName) == false)
            {
                container.Add(targetName, new List<ITarget>());
            }

            container[targetName].Add(target);
        }


        public Task Shutdown()
        {
            return Task.CompletedTask;
            ShutdownCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}
