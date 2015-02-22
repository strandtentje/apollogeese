using System;
using BorrehSoft.ApolloGeese.Duckling;
using System.IO;
using System.Net.Mail;

namespace Networking
{
	public class MailException : Exception
	{
		public MailException (string id) : base(string.Format("it's required to have '{0}' set in either the context or the settings", id))
		{

		}
	}
}

