# Proof of Concept of Application architecture which encapsulated startup steps, ability to inject new steps into startup pipeline of the base class

## The problem with current implementation
- To much details in App.xaml.cs. It is hard to identify high level steps that are run during startup. Lack of encapsulation
- When a module want to run a new code in between base class loading steps, you must modify the base class which affects all modules
- Current application class inherits from WPF application which add unnecessary dependency
- Hard to track down how much loading time single loading step takes
- Synchronous Shutdown which doesn't allow to cancel Shutdown or await async code in shutdown method. Currently this is done in Mail but it is not very elegant and the programmer must remember not to call Applicatin.Current.Shutdown() to have async shutdown

## Proposed Solution

### A new class that doesn't inherit from WPF application

Currently if you want to use Application class in your C# code you must add reference to WPF. This shouldn't be necessary. We can use WPF app only as entry point and call our Application class

```CSharp
    public partial class WpfApp : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var app = new MailApplication();
            app.ShutdownCompleted += App_ShutdownCompleted;
            app.Startup();
            // invoke app.Run() but allow OnStartup to complete first
            Dispatcher.Invoke(() => app.Run());
        }

        private void App_ShutdownCompleted(object sender, EventArgs e)
        {
            Shutdown();
        }
    }
```
### Encapsulating startup steps
We really need high level view of what is the startup sequence is for an application. For that we encapsulate current lines of code in Startup method in individual high level components

```CSharp
    public interface ITarget
    {
        public string Name { get; }
        public void Execute(StartupContext context);
        public bool CanExecute(StartupContext context);
    }

    // example for startup step
    class InitializeSentry : ITarget
    {
        public string Name => "InitializeSentry";

        public bool CanExecute(StartupContext context) => true;

        public void Execute(StartupContext context)
        {
            // initialize sentry code goes here
        }
    }

    class InitializeLogger : ITarget
    {
        public string Name => "InitializeLogger";

        public bool CanExecute(StartupContext context) => true;

        public void Execute(StartupContext context)
        {
            // logger initialization goes here
        }
    }
```

### Injecting startup steps in base class startup sequence

Current implementation show us that we need a base class that defines a startup steps sequence for all modules but modules needs to insert steps in between those steps defined by the base implementation. We would like to achieve that without modification of the code of the base class

```CSharp
    // this is the base class for all modules. Application class implements ExecuteAfter and ExecuteBefore
    // and method for constructing final startup sequence
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

        ... details omitted
    }

    class MailApplication : OfficeSuiteApplication
    {
        protected override void RegisterTargets()
        {
            base.RegisterTargets();

            ExecuteAfter(new UwpStoreInitialize(false), "Startup");
            ExecuteBefore(new InitializeSQLiteDatabase(), "InitializeMsConnect");
            ExecuteBefore(new InitializePartners(), "InitializeMsConnect");
            ExecuteAfter(new LoadMailAccounts(), "InitializeMsConnect");
        }

        ... details omitted
    }
```
The output of this configuration will be the following sequence of startup steps
```
1:Startup
2:RegisterDependencyInjectionTypes
3:UwpStoreInitialize
4:InitializeLogger
5:InitializeSentry
6:InitializeSQLiteDatabase
7:InitializePartners
8:InitializeMsConnect
9:LoadMailAccounts
```
### Audit startup steps

Once we have our code in separate components we can use the telemetry to log how much time each loading step takes and use that information to narrow down performance issues in the individual steps

### Async Shutdown method

This allows the client code to invoke Shutdown method and await it and catch potential `OperationCancelledException` and react to async application shutdown that can be cancelled. May be the name of the method is not good, because someone might argue that Shutdown should not be cancelled. I can agree on that and we can name this method with another name but we must have a cancellable method that signal the app that we want to close it.

### The problem with async methods during startup

My initial thought was that `ITarget` interface should define `ExecuteAsync` method that return `Task`. This will allows us to have the ability to execute async methods during startup. Executing async method means we have long running operation that we don't want to block the main thread. We would like to release the main thread and come back later when the result is ready.

With async method startup sequence will look something like that

```CSharp
    await RegisterDependencyInjectionTypes();
    await UwpStoreInitialize();
    await InitializeLogger();
    await InitializeSentry();
    await InitializeSQLiteDatabase();
    await InitializePartners();
    await InitializeMsConnect();
    await LoadMailAccounts();
    ShowMainWindow();
```
Since this is app startup and the main thread won't do anything else it will idle and wait for worker threads to finish, before moving to the next step. Consider the context switching this will actually make the startup slower than running everything on the main thread. This is the reason I think async startup is unnecessary.

What about if I want to start async operation in a worker thread and don't care in what order this async operation will complete as long as it completed before showing the main window. This is valid concern - for example - may be we would like to start MsConnect initialization but not wait for it to connect and authenticate with MsConnect, but when all startup steps are completed we would like to wait for authenticate with MsConnect before Showing the main window.

This below is potential solution of this problem

```CSharp
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
```
`InitializeMsConnect` startup target register startup task that can later be located by their name from a `StartupContext` and awaited. This way we can have a list of task that we can await before showing the MainWindow if this is necessary. This actually will be usefull even if you don't want the main window to await for those tasks. May be we want to show the main window but the login control to update itself once the MsConnect is authenticated. Somewhere in MainWindow code we can have something like

```CSharp
        private async Task ExecutePostLoadActions()
        {
            var result = await context.Task<int>("MsConnect Authenticated");
            Debug.WriteLine("MSConnect was authenticated");
            UpdateLoginControl();
        }
```

