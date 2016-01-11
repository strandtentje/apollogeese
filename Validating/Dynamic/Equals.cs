using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;

namespace Validating
{
	public class Equals : TwoBranchedService
	{
		public override string Description {
			get {
				return string.Format ("Match value of context variable {0} to {1}",
				                     Test, Subject);
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			Settings ["test"] = defaultParameter;
		}

		public string Test {
			get;
			set;
		}

		public string Subject {
			get;
			set;
		}

		protected override void Initialize (Settings settings)
		{
			this.Test = settings.GetString ("test");
			this.Subject = settings.GetString ("to", settings.GetString("subject", "value"));
		}

		protected virtual Service Compare (object fromValue, object toValue)
		{
			if (fromValue.Equals (toValue)) {
				return Successful;
			} else {
				return Failure;
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			Service conclusion = Failure;

			object testPattern, testSubject;
			if (parameters.TryGetFallback (this.Test, out testPattern)) {
				if (parameters.TryGetFallback (this.Subject, out testSubject)) {
					conclusion = Compare (testPattern, testSubject);
				}
			}

			return conclusion.TryProcess (parameters);
		}
	}
}

