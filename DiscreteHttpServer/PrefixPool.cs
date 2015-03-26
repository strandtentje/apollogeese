using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscreteHttpServer
{
    class PrefixPool
    {
        internal void Bind(string p, Action<RequestHeader> IncomingRequestHandler)
        {
            throw new NotImplementedException();
        }

        internal void Bind(string p, Action<RequestHeader, System.Net.Sockets.TcpClient> IncomingRequestHandler)
        {
            throw new NotImplementedException();
        }
    }
}
