using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Bootstrap.Docker;
using Akka.Configuration;

namespace SimpleCluster_NetCore20_Akka138
{
	public class SimpleClusterService
	{
		public readonly ManualResetEvent ManualResetEvent = new ManualResetEvent(false);
		protected ActorSystem ClusterSystem;

		public Task WhenTerminated => ClusterSystem.WhenTerminated;

		public bool Start()
		{
		
			var hocon = ConfigurationFactory.ParseString(File.ReadAllText("simplecluster.hocon"));

			ClusterSystem = ActorSystem.Create("ClusterSystem", hocon.BootstrapFromDocker());
			ClusterSystem.ActorOf(SimpleClusterListener.Props, "clusterListener");

			Task.Run(async () =>
			{
				while (true)
				{
					var cluster = Akka.Cluster.Cluster.Get(ClusterSystem);
					Console.WriteLine($"Members: {cluster.State.Members.Count}");
					Console.WriteLine($"Unreachable: {cluster.State.Unreachable.Count}");
					Console.WriteLine($"Leader: {cluster.State.Leader?.Host ?? "no leader"}");

					foreach (var stateMember in cluster.State.Members)
					{
						Console.WriteLine($"{stateMember.Address}: {stateMember.Status}");
					}

					await Task.Delay(new TimeSpan(0, 0, 0, 5));
				}
			});

			return true;
		}

		public void Stop()
		{
			// return CoordinatedShutdown.Get(ClusterSystem).Run();

			var cluster = Akka.Cluster.Cluster.Get(ClusterSystem);
			cluster.RegisterOnMemberRemoved(() => MemberRemoved(ClusterSystem));
			cluster.Leave(cluster.SelfAddress);

			ManualResetEvent.WaitOne();
		}

		private async void MemberRemoved(ActorSystem actorSystem)
		{
			await actorSystem.Terminate();
			ManualResetEvent.Set();
		}
	}
}