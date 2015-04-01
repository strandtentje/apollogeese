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
        public string Protocol { get; private set; }
        public List<string> Sections { get; private set; }

        private static delegate bool Consumer(StringBuilder, char);

        internal static RequestUri FromStream(Stream stream)
        {
            RequestUri uri = new RequestUri();
            StringBuilder token = new StringBuilder(); char character;
            
            Consumer consume = uri.ConsumeProtocol;
            Queue<Consumer> consumeStack = GetConsumeQueue(
                uri.ConsumeProtocol, uri.ConsumeLeadingSlash, uri.ConsumePath);

            for (uri.TotalLength = 0; uri.TotalLength < MaxLength; uri.TotalLength++)
            {
                character = (char)stream.ReadByte();

                if (consume(token, character))
                {

                }


                if ((stage == UriStage.Protocol) && (character == ':'))
                {
                    uri.Protocol = token.ToString();
                    token.Clear();
                    stage == UriStage.LeadingSlash;
                }
                
            }
        }

        private static Queue<Consumer> GetConsumeQueue(params Consumer[] consumers) {
            return new Queue<Consumer>(consumers);
        }

        private bool ConsumeProtocol(StringBuilder data, char character)
        {
            if (character == ':')
            {
                Protocol = data.ToString();
                data.Clear();
                return true;
            }

            data.Append(character);

            return false;
        }

        private bool ConsumeLeadingSlash(StringBuilder data, char character) 
        {
            if (character == '/') {
                return true;
            }

            data.Append(character);

            return true;
        }

        private bool ConsumePath(StringBuilder data, char character) {

        }
    }
}
