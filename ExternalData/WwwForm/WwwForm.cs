using System;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;

namespace ExternalData
{
	public class WwwForm : ExternalDataService
	{
		public override string Description {
			get {
				return "URL Encoded data parser";
			}
		}

		Service None {
			get;
			set;
		}

		Service Iterator {
			get;
			set;
		}

		Service Mapped {
			get;
			set;
		}

		bool DoMapping {
			get;
			set;
		}

		bool DoIterate {
			get;
			set;
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			base.HandleBranchChanged (sender, e);

			if (e.Name == "none")
				this.None = e.NewValue;
			else if (e.Name == "iterator") {
				this.Iterator = e.NewValue;
				this.DoIterate = this.Iterator != null;
			}
			else if (e.Name == "mapped") {
				this.Mapped = e.NewValue;
				this.DoMapping = this.Mapped != null;
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			TextReader reader;
			bool success;

			if (success = TryGetDatareader (parameters, null, out reader)) {
				// #yolo
				string fullData = reader.ReadToEnd ();

				string[] pairs = fullData.Split ('&');

				WwwFormInteraction inputs = new WwwFormInteraction (parameters);

				foreach (string pair in pairs) {
					WwwInputInteraction input = new WwwInputInteraction (pair, parameters);
					if (this.DoIterate) 
						success &= this.Iterator.TryProcess (input);
					if (this.DoMapping)	
						inputs.Register (input);
					if (this.Branches.Has (input.Name))
						success &= this.Branches [input.Name].TryProcess (input);
				}

				if (this.DoMapping)
					success &= this.Mapped.TryProcess (inputs);

			}

			return success;
		}
	}
}

