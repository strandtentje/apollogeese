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
			List<object> definedFields = (List<object>)modSettings ["fields"];
			myBranches.AddRange (definedFields.ToStringArray ());
		}

		protected override bool Process (IInteraction parameters)
		{
			Map<object> incoming = new Map<object> ();
			IInteraction tmpParam = parameters.Clone ();
			bool success = true;

			foreach (string branchName in myBranches) {
				if (success)
				{
					success &= RunBranch (branchName, tmpParam);
				}
			}


			return false;
		}
	}
}

