using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations
{
	public class ControlException : Exception
	{
		public Cause ExceptionCause {
			get;
			private set;
		}

		public string Key {
			get;
			private set;
		}

		public ControlException (ControlException.Cause cause, string key) : base(
			string.Format ("Code {0} on key {1}", (int)cause, key))
		{
			this.ExceptionCause = cause;
			this.Key = key;
		}

		public enum Cause : int
		{
			Unparseable = 11,

			IntParse = 1,
			TypeMismatch = 2,
			NoCandidate = 3,
			NoService = 4,
			NoBranchSupplied = 5,
			Uninitialized = 6,			
			NoServiceName = 7,
			BadService = 8,
			NoMapFound = 9,
			Exception = 10
		}
	}
}

