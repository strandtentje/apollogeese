using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DiscreteHttpServer
{
    class RequestLine
    {
        public RequestMethod Method { get; private set; }
        public RequestUri URL { get; private set; }
        public RequestVersion Version { get; private set; }

        public RequestLine()
        {

        }

        internal static RequestLine FromStream(Stream stream)
        {
            RequestLine line = new RequestLine();

            line.Method = RequestMethod.FromStream(stream);
            line.URL = RequestUri.FromStream(stream);
            line.Version = RequestVersion.FromStream(stream);

            return line;
        }
    }
}
