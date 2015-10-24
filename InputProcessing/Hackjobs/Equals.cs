using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;

namespace InputProcessing
{
	public class Equals : TwoBranchedService
	{
		public override string Description {
			get {
				return string.Format ("Match value of context variable {0} to {1}",
				                     From, To);
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			Settings ["from"] = defaultParameter;
		}

		public string From {
			get;
			set;
		}

		public string To {
			get;
			set;
		}

		protected override void Initialize (Settings settings)
		{
			this.From = settings.GetString ("from");
			this.To = settings.GetString ("to");
		}

		protected override bool Process (IInteraction parameters)
		{
			Service conclusion = Failure;

			object fromValue, toValue;
			if (parameters.TryGetFallback (this.From, out fromValue)) {
				if (parameters.TryGetFallback (this.To, out toValue)) {
					if (fromValue.Equals (toValue)) {
						conclusion = Successful;
					}
				}
			}

			return conclusion.TryProcess (parameters);
		}
	}
}

