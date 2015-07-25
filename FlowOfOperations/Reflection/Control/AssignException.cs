using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	public class AssignException : Exception
	{
		public Cause ExceptionCause {
			get;
			private set;
		}

		public string Key {
			get;
			private set;
		}

		public AssignException (AssignException.Cause cause, string key) : base(
			string.Format ("Code {0} on key {1}", (int)cause, Key))
		{
			this.ExceptionCause = cause;
			this.Key = key;
		}

		public enum Cause : int
		{
			IntParse = 1,
			TypeMismatch = 2,
			NoCandidate = 3,
			NoService = 4,
			NoBranchSupplied = 5
		}
	}
}

