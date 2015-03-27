using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DiscreteHttpServer
{
    class RequestHead
    {
        public RequestLine RequestLine { get; private set; }

        public RequestHead()
        {

        }

        internal static RequestHead FromStream(Stream stream)
        {
            RequestHead newHead = new RequestHead();
            RequestHeader newHeader;

            newHead.RequestLine = RequestLine.FromStream(stream);

            newHeader = RequestHeader.FromStream(stream);

            while (newHeader.Valid)
            {
                newHead.Headers.Add(newHeader);
                newHeader = RequestHeader.FromStream(stream);
            }

            return newHead;
        }
    }
}
