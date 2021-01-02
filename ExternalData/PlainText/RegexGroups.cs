using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;
using System.Text.RegularExpressions;
using System.Globalization;
using BorrehSoft.Utilities.Parsing;
using System.IO;

namespace ExternalData
{
	/// <summary>
	/// Regex field reader. This is exactly what you think it is.
	/// A dirty hack.
	/// </summary>
	public class RegexGroups : NameValueService
	{
		private Regex matcher;
		private Service successful;

		public override string Description {
			get {
				return string.Join(", ", matcher.GetGroupNames());
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
            base.HandleBranchChanged(sender, e);
			if (e.Name == "successful")
				successful = e.NewValue;
		}

		protected override void Initialize (Settings modSettings)
		{
            base.Initialize(modSettings);
			matcher = new Regex(modSettings["regex"] as String);
		}

		protected override bool Process (IInteraction parameters)
		{
			TextReader reader;
			bool success;

			if (success = TryGetDatareader (parameters, null, out reader)) {
				string fullData = reader.ReadToEnd ();
				Match results = matcher.Match (fullData);

                if (!results.Success) return None.TryProcess(parameters);

				SimpleInteraction inputs = new SimpleInteraction (parameters);

				foreach (string groupName in matcher.GetGroupNames()) {
					NameValueInteraction foundGroup = new NameValueInteraction (parameters) {
						Name = groupName,
						Value = Parser.GetBestPossible (results.Groups [groupName].Value)
					};

					success &= TryReportPair (inputs, foundGroup);
				}

				if (this.DoMapping)
					success &= this.Mapped.TryProcess (inputs);
			}

			return success;
		}
	}
}

