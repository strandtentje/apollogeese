using System;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	public static class LoggingExceptionHandler
	{
		public static void Handle(Service origin, Exception cause) {
			string message = "";

			if (origin.InitErrorMessage.Length > 0) {
				message = string.Format (
					"Already initialized badly with the message on line {3}:\n" +
					"{1}.\n" +
					"The message for this failure was:\n{2}",
					origin.Description, origin.InitErrorMessage, 
					cause.Message, origin.ConfigLine);
			} else {
				message = cause.Message;
			}

			Secretary.Report (0, string.Format (
				"Processing for Service {0} " +
				"failed with the following message: \n{1}\n" +
				"on line {2}", origin.Description, message,
				origin.ConfigLine));

			for(Exception inner = cause; inner != null; inner = inner.InnerException)
				Secretary.Report(0, "Inner: ", inner.Message);
		}
	}
}

