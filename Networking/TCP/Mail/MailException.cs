using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;
using System.Net.Mail;
using System.Collections.Generic;

namespace Networking
{
	public class MailException : Exception
	{
		public MailException (string id) : base(string.Format(
			"it's required to have '{0}' set in either the context or the settings", id))
		{

		}

		public MailException (IEnumerable<string> servers) : base(string.Format(
			"None of these smtp-servers work: ", string.Join(", ", servers))) 
		{

		}
	}
}

