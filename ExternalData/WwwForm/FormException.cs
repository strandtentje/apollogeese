using System;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;
using BorrehSoft.Utilities.Collections.Settings;
using System.Collections.Generic;
using BorrehSoft.Utilities.Log;
using BorrehSoft.Utilities.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Parsing;
using System.Web;

namespace ExternalData
{
	class FormException : Exception
	{
		public FormException (string messageFormat, string parameter) : base(string.Format(messageFormat, parameter))
		{
			
		}

		public FormException (string message): base(message)
		{
		}
	}

}
