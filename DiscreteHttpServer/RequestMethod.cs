using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DiscreteHttpServer
{

    public class RequestMethod
    {
        // public enum Methods { OPTIONS, GET, HEAD, POST, PUT, DELETE, TRACE, CONNECT };

        public RequestMethod(string methodName)
        {
            this.MethodName = methodName;
        }

        public string MethodName { get; private set; }

        public static RequestMethod OPTIONS = new RequestMethod("OPTIONS");
        public static RequestMethod GET =     new RequestMethod("GET");
        public static RequestMethod HEAD =    new RequestMethod("HEAD");
        public static RequestMethod POST =    new RequestMethod("POST");
        public static RequestMethod PUT =     new RequestMethod("PUT");
        public static RequestMethod DELETE =  new RequestMethod("DELETE");
        public static RequestMethod TRACE =   new RequestMethod("TRACE");
        public static RequestMethod CONNECT = new RequestMethod("CONNECT");

        internal static RequestMethod FromStream(Stream stream)
        {
            RequestMethod method; StringBuilder methodBuilder = new StringBuilder();
            char currentCharacter;

            for (int i = 0; i < 8; i++)
            {
                currentCharacter = (char)stream.ReadByte();
                if (currentCharacter == ' ') break;
                else methodBuilder.Append(currentCharacter);
            }

            string methodString = methodBuilder.ToString();

            try
            {
                method = (RequestMethod)typeof(RequestMethod).GetProperty(
                    methodString, System.Reflection.BindingFlags.Static).GetValue(null, null);
            }
            catch(Exception ex)
            {
                throw new BadMethodException(ex, methodString);
            }
            
            return method;
        }
    }
}
