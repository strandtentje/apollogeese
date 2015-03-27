using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscreteHttpServer
{
    class RequestHeader
    {
        private System.Net.Sockets.NetworkStream networkStream;

        public RequestHeader(System.Net.Sockets.NetworkStream networkStream)
        {
            // TODO: Complete member initialization
            this.networkStream = networkStream;
        }
    }
}
