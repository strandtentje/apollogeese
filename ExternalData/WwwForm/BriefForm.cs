using System;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;
using BorrehSoft.Utilities.Collections.Settings;
using System.Collections.Generic;
using BorrehSoft.Utilities.Log;
using BorrehSoft.Utilities.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Parsing;
using System.Web;

namespace ExternalData
{
	public abstract class BriefForm<T> : HttpForm<T>
	{
		bool Immediate;
		private bool BypassFieldlist;

		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);
			this.Immediate = settings.GetBool ("immediate", false); 
			this.BypassFieldlist = settings.GetBool("bypass", false);
		}

		public override bool CheckMimetype (string mimeType)
		{
			string[] specifiers = mimeType.Split (';');

			string mime = specifiers [0];
			if (specifiers.Length > 1) {
				string encoding = specifiers [1];
				if (!encoding.ToLower ().EndsWith (this.Encoding.HeaderName)) {
					throw new Exception ("Encoding mismatch (temporary)");
				}
			}

			return mime == "application/x-www-form-urlencoded";
		}

		Service ClientFailure;
		Service ServerFailure;

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "clientfailure") this.ClientFailure = e.NewValue;
			else if (e.Name == "serverfailure") this.ServerFailure = e.NewValue;
			else base.HandleBranchChanged(sender, e);
		}

		protected override bool Process (IInteraction parameters)
		{
			bool success = true;
			TextReader urlDataReader = null;
			SimpleInteraction valuesByName = new SimpleInteraction (parameters);
			Map<IInteraction> inputInteractionsByName = new Map<IInteraction> ();

			try {
				if (!TryGetDatareader (parameters, null, out urlDataReader)) {
					throw new FormException("Getting data reader failed");
				}

				if (!this.ParserRunner.TryRun (urlDataReader, delegate(string name, T value) {
					var inputInteractions = new InputInteraction<T> (name, value, parameters);

					if (BypassFieldlist || StringFieldWhiteList.Contains (inputInteractions.Name)) {
						if (Immediate)
							success &= TryReportPair (valuesByName, inputInteractions);
						else {
							if (this.DoMapping)
								valuesByName [inputInteractions.Name] = inputInteractions.Value;
							inputInteractionsByName [inputInteractions.Name] = inputInteractions;
						}
					}
				})) {
					throw new FormException("Parser-runner timed out");
				}
			} catch(FormException ex) {
				return ClientFailure.TryProcess(parameters);
			} catch(Exception ex) {
				return ServerFailure.TryProcess(parameters);
			}

			foreach (string fieldName in this.StringFieldWhiteList) {
				IInteraction currentField;

				if (!inputInteractionsByName.Has (fieldName)) {
					inputInteractionsByName [fieldName] = new InputInteraction<T> (fieldName, default(T), parameters);
				}
			
				currentField = inputInteractionsByName [fieldName];

				success = success && (!Branches.Has (fieldName) || Branches [fieldName].TryProcess (currentField));
				success = success && (!DoIterate || this.Iterator.TryProcess (currentField));
			}			

			success = success && (!this.DoMapping || this.Mapped.TryProcess (valuesByName));
	
			return success;
		}
	}
}
