using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.BorrehSoft.Utensils.Collections;

namespace Datatables
{
	public class InputForm : Service
	{
		private static readonly string[] permanentBranches = new string[] { "succes" };
		private List<string> myBranches = new List<string>();
		private string correctMethod;

		public InputForm() {
			myBranches.AddRange (permanentBranches);
		}

		public override string[] AdvertisedBranches {
			get {
				return myBranches.ToArray();
			}
		}

		public override string Description {
			get {
				return "Module that loads User Input into the Parameters.";
			}
		}

		protected override void Initialize (Settings modSettings)
		{
			correctMethod = (string)modSettings ["method"];

			List<object> definedFields = (List<object>)modSettings ["fields"];
			myBranches.AddRange (definedFields.ToStringArray ());
		}

		protected override bool Process (IInteraction uncastParameters)
		{
			Map<object> incoming = new Map<object> ();
			WebformInteraction parameters = 
				new WebformInteraction (
					(IHttpInteraction)uncastParameters,
					correctMethod);

			bool success = true;

			foreach (string branchName in myBranches) 
				success &= RunBranch (branchName, parameters);

			return success;
		}
	}
}
