using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DiscreteHttpServer
{
    class RequestLine
    {
        public static int MaxLength { get; set; }
        public int Length { get; private set; }
        public RequestMethod Method { get; private set; }

        public RequestLine()
        {

        }

        internal static RequestLine FromStream(Stream stream)
        {
            RequestLine line = new RequestLine();

            line.Method = RequestMethod.FromStream(stream);

            StreamReader reader = new StreamReader(stream);
            



            if (requestLineSections.Length != 3)
                throw new TooManySectionsException();

            string 
                methodString = requestLineSections[0],
                uriString = requestLineSections[1],
                versionString = requestLineSections[2];

            if (!Enum.TryParse<RequestMethod>(methodString, out line.requestMethod))
                throw new BadMethodException();


        }
    }
}
