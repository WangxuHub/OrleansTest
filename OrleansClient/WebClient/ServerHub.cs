using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace WebClient
{
    public class ServerHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }

        public void SendMsg(string message)
        {
            //调用所有客户端的sendMessage方法  
            Clients.All.sendMessage(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message);
        }

        public override Task OnConnected()
        {
            System.Diagnostics.Trace.WriteLine("客户端连接成功！");
            return base.OnConnected();
        }
    }
}