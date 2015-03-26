using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.ApolloGeese.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscreteHttpServer
{
    class HttpInteraction : QuickInteraction, IHttpInteraction, IDisposable
    {
        private RequestHeader body;
        private System.Net.Sockets.TcpClient socket;

        public HttpInteraction(RequestHeader body, System.Net.Sockets.TcpClient socket)
        {
            // TODO: Complete member initialization
            this.body = body;
            this.socket = socket;
        }

        public void Dispose()
        {

        }
    }
}
