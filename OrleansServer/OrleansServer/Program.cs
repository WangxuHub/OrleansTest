using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //var config = Orleans.Runtime.Configuration.ClusterConfiguration.LocalhostPrimarySilo();
            
            var host = new Orleans.Runtime.Host.SiloHost("Default");
            host.LoadOrleansConfig();

            host.InitializeOrleansSilo();
            host.StartOrleansSilo();


            //var config = Orleans.Runtime.Configuration.ClientConfiguration.LocalhostSilo();
            //GrainClient.Initialize(config);

            //var userID = Guid.NewGuid();
            //IHello pub = Orleans.GrainClient.GrainFactory.GetGrain<IHello>(userID);

            while (true)
            {
                var msg = Console.ReadLine();

                if(msg=="quit")
                {
                    break;
                }
                //pub.SendUpdateMessage(msg);
            }

            host.StopOrleansSilo();

            Console.ReadLine();
        }
    }

    public interface IGameGrain:Orleans.IGrainWithGuidKey
    {

    }
    
    class HelloGrain : Grain, IHello
    {
        private ObserverSubscriptionManager<IChat> _subsManager;

        public override async Task OnActivateAsync()
        {
            // We created the utility at activation time.
            _subsManager = new ObserverSubscriptionManager<IChat>();
            await base.OnActivateAsync();
        }

        // Clients call this to subscribe.
        public Task Subscribe(IChat observer)
        {
            _subsManager.Subscribe(observer);
            return TaskDone.Done;
        }

        //Also clients use this to unsubscribe themselves to no longer receive the messages.
        public Task UnSubscribe(IChat observer)
        {
            _subsManager.Unsubscribe(observer);
            return TaskDone.Done;
        }

        public Task SendUpdateMessage(string message)
        {
            _subsManager.Notify(s => s.ReceiveMessage(message));
            return TaskDone.Done;
        }
    }

    public interface IHello:Orleans.IGrainWithGuidKey
    {
        Task Subscribe(IChat observer);

        Task SendUpdateMessage(string message);
    }

    public interface IChat : Orleans.IGrainObserver
    {
        void ReceiveMessage(string message);
    }

}
