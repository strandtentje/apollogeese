using System;
using BorrehSoft.Utilities.Log;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	public static class LoggingExceptionHandler
	{
		public static void Handle(Service cause, IInteraction context, Exception problem) {
			string message = "";

			if (cause.InitErrorMessage.Length > 0) {
				message = string.Format (
					"Already initialized badly with the message on line {3}:\n" +
					"{1}.\n" +
					"The message for this failure was:\n{2}",
					cause.Description, cause.InitErrorMessage, 
					problem.Message, cause.ConfigLine);
			} else {
				message = problem.Message;
			}

			Secretary.Report (0, string.Format (
				"Processing for Service {0} " +
				"failed with the following message: \n{1}\n" +
				"on line {2}", cause.Description, message,
				cause.ConfigLine));

			for(Exception inner = problem; inner != null; inner = inner.InnerException)
				Secretary.Report(0, "Inner: ", inner.Message);
		}
	}
}

