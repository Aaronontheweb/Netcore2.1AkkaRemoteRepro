using Akka.Actor;
using Akka.Cluster;
using Akka.Event;

namespace SimpleCluster_NetCore20_Akka138
{
	public class SimpleClusterListener : UntypedActor
	{
		protected ILoggingAdapter Log = Context.GetLogger();
		protected Cluster Cluster = Cluster.Get(Context.System);

		/// <summary>
		/// Need to subscribe to cluster changes
		/// </summary>
		protected override void PreStart()
		{
			Cluster.Subscribe(Self, ClusterEvent.InitialStateAsEvents, typeof(ClusterEvent.IMemberEvent), typeof(ClusterEvent.UnreachableMember));
		}

		/// <summary>
		/// Re-subscribe on restart
		/// </summary>
		protected override void PostStop()
		{
			Cluster.Unsubscribe(Self);
		}

		protected override void OnReceive(object message)
		{
		    switch (message)
		    {
		        case ClusterEvent.MemberUp up:
		            var mem = up;
		            Log.Info("Member is Up: {0}", mem.Member);
		            break;

		        case ClusterEvent.UnreachableMember _:
		            var unreachable = (ClusterEvent.UnreachableMember)message;
		            Log.Info("Member detected as unreachable: {0}", unreachable.Member);
		            break;

		        case ClusterEvent.MemberRemoved _:
		            var removed = (ClusterEvent.MemberRemoved)message;
		            Log.Info("Member is Removed: {0}", removed.Member);
		            break;

		        case ClusterEvent.IMemberEvent _:
		            //IGNORE                
		            break;

		        case ClusterEvent.CurrentClusterState _:
		            break;

		        default:
		            Unhandled(message);
		            break;
		    }
		}

	    public static Props Props => Props.Create(typeof(SimpleClusterListener));

	}
}