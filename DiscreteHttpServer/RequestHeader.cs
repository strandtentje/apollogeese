﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DiscreteHttpServer
{
    class Header
    {
        internal static Header FromStream(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);

            reader.ReadLine();
        }

        public bool Valid { get; set; }
    }
}
