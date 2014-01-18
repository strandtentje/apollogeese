using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Settings;
using BorrehSoft.Utensils;

namespace Datatables
{
	public class InputForm : Service
	{
		private static readonly string[] permanentBranches = new string[] { "succes" };
		private List<string> myBranches = new List<string>();

		public InputForm() {
			myBranches.AddRange (permanentBranches);
		}

		public override string[] AdvertisedBranches {
			get {
				return myBranches;
			}
		}

		public override string Description {
			get {
				return "Module that loads User Input into the Parameters.";
			}
		}


		protected override void Initialize (Settings modSettings)
		{
			List<object> definedFields = (List<object>)modSettings ["fields"];

			myBranches.Add (definedFields.ToStringArray ());

		}

		protected override bool Process (Interaction parameters)
		{

		}
	}
}

