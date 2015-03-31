using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscreteHttpServer
{
    class BadMethodException : Exception
    {
        public BadMethodException(Exception ex, string method) : 
            base(string.Format("HTTP method '{0}' not found", method), ex)
        {

        }
    }
}
