using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System.Text.RegularExpressions;
using System.Globalization;
using BorrehSoft.Utensils.Parsing;
using System.IO;

namespace ExternalData
{
	/// <summary>
	/// Regex field reader. This is exactly what you think it is.
	/// A dirty hack.
	/// </summary>
	public class RegexFieldReader : NameValueService
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
			if (e.Name == "successful")
				successful = e.NewValue;
		}

		protected override void Initialize (Settings modSettings)
		{
			matcher = new Regex(modSettings["regex"] as String);
		}

		protected override bool Process (IInteraction parameters)
		{
			TextReader reader;
			bool success;

			if (success = TryGetDatareader (parameters, null, out reader)) {
				string fullData = reader.ReadToEnd ();
				Match results = matcher.Match (fullData);

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

