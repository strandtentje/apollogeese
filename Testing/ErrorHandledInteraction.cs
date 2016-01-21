using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Text;

namespace Testing
{
	public class ErrorHandledInteraction : SimpleInteraction
	{
		public ErrorHandledInteraction (
			Service cause, IInteraction context, Exception problem
		) : base(context) {
			this ["errorfile"] = cause.ConfigLine;
			this ["errormessage"] = problem.Message;
			this ["errorcause"] = cause.Description;
			this ["errorinitializing"] = cause.InitErrorMessage;
			this ["errorstack"] = problem.StackTrace;
		}
	}
}

