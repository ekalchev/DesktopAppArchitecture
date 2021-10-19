using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationPoc
{
    public class StartupContext
    {
        private Dictionary<string, object> startupTasks = new Dictionary<string, object>();
        public void CreateTask<T>(string name)
        {
            startupTasks.Add(name, new TaskCompletionSource<T>());
        }

        public void ResolveTask<T>(string name, Task<T> task)
        {
            var tcs = startupTasks[name];
            ResolveTcs((TaskCompletionSource<T>)tcs, task);
        }

        private async Task ResolveTcs<T>(TaskCompletionSource<T> tcs, Task<T> task)
        {
            try
            {
                await task;
                tcs.TrySetResult(task.Result);
            }
            catch(OperationCanceledException)
            {
                tcs.TrySetCanceled();
            }
            catch(Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }

        public Task<T> Task<T>(string name)
        {
            return ((TaskCompletionSource<T>)startupTasks[name]).Task;
        }
    }
}
