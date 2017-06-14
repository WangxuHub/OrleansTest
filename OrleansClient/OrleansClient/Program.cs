using OrleansServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Orleans.GrainClient.Initialize();

            Console.WriteLine("pub or sub?");
            var clientType = Console.ReadLine();

            var key = Guid.Parse("a91034ae-9bb4-4e1a-b6bd-909d7ef2f330");

            if (clientType == "pub")
            {
                var friend = Orleans.GrainClient.GrainFactory.GetGrain<IHello>(key);

                while (true)
                {
                    Console.Write("input");
                    var msg = Console.ReadLine();
                    if (msg == "quit")
                    {
                        break;
                    }

                    friend.SendUpdateMessage(msg);
                }
            }
            else
            {
                var friend = Orleans.GrainClient.GrainFactory.GetGrain<IHello>(key);
                Chat c = new Chat();

                var obj = Orleans.GrainClient.GrainFactory.CreateObjectReference<IChat>(c).Result;

                friend.Subscribe(obj).Wait();
            }


            Console.ReadLine();
        }

        public class Chat : IChat
        {
            public void ReceiveMessage(string message)
            {
                Console.WriteLine(message);
            }
        }
    }

}


namespace OrleansServer
{
    public interface IChat : Orleans.IGrainObserver
    {
        void ReceiveMessage(string message);
    }

    public interface IHello : Orleans.IGrainWithGuidKey
    {
        Task Subscribe(IChat observer);

        Task SendUpdateMessage(string message);
    }

    public interface IUserService : Orleans.IGrainWithIntegerKey
    {
        Task<bool> Exist(string mobileNumber);
    }

    public interface IGameGrain : Orleans.IGrainWithGuidKey
    {

    }

    //an example of a Grain Interface
    public interface IPlayerGrain : Orleans.IGrainWithGuidKey
    {
        Task<IGameGrain> GetCurrentGame();
        Task JoinGame(IGameGrain game);
        Task LeaveGame(IGameGrain game);
    }
}