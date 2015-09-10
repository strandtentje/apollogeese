using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DiscreteHttpServer
{
    class RequestLine
    {
		public HttpMethod Method { get; private set; }
        public Uri URL { get; private set; }
        public string Version { get; private set; }

        public RequestLine()
        {

        }

		class RequestLineException : Exception
		{
			public RequestLineException(string message) : base(message) {}
		}

        internal static RequestLine FromStream(Stream stream)
        {
            RequestLine line = new RequestLine();

			StreamReader versionReader = new StreamReader (stream);

			string[] splitLine = versionReader.ReadLine ().Split(' ');

			if (splitLine.Length == 3) {
				line.Method = (HttpMethod)Enum.Parse (typeof(HttpMethod), splitLine [0]);
				line.URL = new Uri (splitLine [1]);
				line.Version = splitLine [2];
			} else {
				throw new RequestLineException ("Malformed line");
			}

            return line;
        }
    }
}
