using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DiscreteHttpServer
{
    public class HttpServer : Service
    {
        private static PrefixPool prefixPoolInstance = null;
        private Service httpBranch;

        private static PrefixPool Pool
        {
            get
            {
                if (prefixPoolInstance == null)
                    prefixPoolInstance = new PrefixPool();

                return prefixPoolInstance;
            }
        }

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
            foreach (object prefix in ((List<object>)modSettings["prefixes"]))
                Pool.Bind((string)prefix, IncomingRequestHandler);            
        }

        private void IncomingRequestHandler(RequestHeader body, TcpClient socket)
        {
            HttpInteraction parameters = new HttpInteraction(
                body, socket);

            Process(parameters);

            parameters.Dispose();
        }

        protected override bool Process(IInteraction parameters)
        {
            return httpBranch.TryProcess(parameters);
        }
    }
}
