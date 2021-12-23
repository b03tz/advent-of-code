using System;
using System.Threading.Tasks;
using WatsonWebsocket;

namespace SocketClient
{
    public class SocketClient
    {
        private readonly WatsonWsClient client;
        
        public SocketClient()
        {
            client = new WatsonWsClient("localhost", 29463, false);
            
            this.client.ServerConnected += this.ServerConnected;
            this.client.ServerDisconnected += this.ServerDisconnected;
            this.client.MessageReceived += this.MessageReceived;
            
            client.Start();
        }

        public void Send()
        {
            client.SendAsync("TEST");
        }

        public void CreateObject(CreateObjectMessage message)
        {
            
        }

        private void MessageReceived(object? sender, MessageReceivedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void ServerDisconnected(object? sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Console.WriteLine("Socket disconneccted!");
        }

        private void ServerConnected(object? sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Console.WriteLine("Socket conneccted!");
        }
    }

    public class CreateObjectMessage
    {
    }
}