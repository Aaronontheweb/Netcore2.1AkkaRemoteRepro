using System;
using System.Runtime.Loader;
using System.Threading;

namespace SimpleCluster_NetCore20_Akka138
{
	class Program
	{
		private static SimpleClusterService _simpleClusterService;
		public static ManualResetEvent Shutdown = new ManualResetEvent(false);
		public static ManualResetEventSlim Complete = new ManualResetEventSlim();

		static void Main()
		{

			Init();
			RunSingle();

			Console.WriteLine("Exiting");
			Complete.Set();
		}

		private static void Init()
		{
			Console.WriteLine("Initializing");
			AssemblyLoadContext.Default.Unloading +=
				SigTermEventHandler; //register sigterm event handler. Don't forget to import System.Runtime.Loader!
			Console.CancelKeyPress += CancelHandler; //register sigint event handler
		}

		private static void CancelHandler(object sender, ConsoleCancelEventArgs e)
		{
			Console.WriteLine("In CancelHandler...");
		}

		private static void SigTermEventHandler(AssemblyLoadContext obj)
		{
			Console.WriteLine("Shutting down in response to SIGTERM");
			_simpleClusterService.Stop();

			Shutdown.Set();
			Complete.Wait();
		}

		private static void RunSingle()
		{
			Console.WriteLine("RunSingle");
			_simpleClusterService = new SimpleClusterService();
			_simpleClusterService.Start();
			Shutdown.WaitOne();
		}
	}
}