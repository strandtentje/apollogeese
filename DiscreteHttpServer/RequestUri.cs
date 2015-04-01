using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DiscreteHttpServer
{
    class RequestUri
    {
        public static int MaxLength = 8096;
        public int TotalLength { get; private set; }

        internal static RequestUri FromStream(Stream stream)
        {
            RequestUri uri = new RequestUri();

            for (uri.TotalLength = 0; uri.TotalLength < MaxLength; uri.TotalLength++)
            {

            }
        }
    }
}
