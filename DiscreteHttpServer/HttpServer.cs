using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BorrehSoft.ApolloGeese.CoreTypes;

namespace DiscreteHttpServer
{
    public class HttpServer : Service
    {
        private TcpListener listener;
        private Service httpBranch;
        private int port;

        public override string Description
        {
            get { return "Discrete http server on TCP socket"; }
        }

        protected override void HandleBranchChanged(object sender, ItemChangedEventArgs<Service> e)
        {
            if (e.Name == "http")
                httpBranch = e.NewValue;
        }

        protected override void Initialize(Settings modSettings)
        {
            listener = new TcpListener(IPAddress.Any, modSettings.GetInt("port", 8000));

            listener.BeginAcceptTcpClient(SetIncomingClient, listener);
        }

        /// <summary>
        /// TCP connection on specified port.
        /// </summary>
        /// <param name="ar">Async Result</param>
        private void SetIncomingClient(IAsyncResult ar)
        {
            TcpClient newClient = listener.EndAcceptTcpClient(ar);
            listener.BeginAcceptTcpClient(SetIncomingClient, ar.AsyncState);

            RequestHead requestHeader = RequestHead.FromStream(newClient.GetStream());

            Process(new HttpInteraction(requestHeader, newClient));
        }

        protected override bool Process(IInteraction parameters)
        {
            return httpBranch.TryProcess(parameters);
        }
    }
}
