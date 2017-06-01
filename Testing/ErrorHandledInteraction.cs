using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Text;
using System.Collections.Generic;
using System.Linq;

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

			StringBuilder inners = new StringBuilder(), datas = new StringBuilder();
			for (Exception ex = problem; ex != null; ex =ex.InnerException) {
				inners.AppendFormat("{0},", ex.Message);
				foreach (var key in ex.Data.Keys) {
					datas.AppendFormat("{0}={1},", key.ToString(), ex.Data[key].ToString());
				}
			}

			this["inner"] = inners.ToString();
			this["data"] = datas.ToString();
		}
	}
}

