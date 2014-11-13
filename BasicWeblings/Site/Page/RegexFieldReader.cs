using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System.Text.RegularExpressions;
using System.Globalization;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Page
{
	/// <summary>
	/// Regex field reader. This is exactly what you think it is.
	/// A dirty hack.
	/// </summary>
	public class RegexFieldReader : Service
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
			IIncomingBodiedInteraction incoming = (IIncomingBodiedInteraction)parameters;
			QuickInteraction parsed = new QuickInteraction (incoming);
			Match results = matcher.Match (incoming.IncomingBody.ReadToEnd ());

			foreach (string groupName in matcher.GetGroupNames()) {
				int intValue; long longValue; double floatValue; string stringValue = results.Groups [groupName].Value;

				if (int.TryParse(stringValue, out intValue)) {
				    parsed [groupName] = intValue;
				} else if (long.TryParse(stringValue, out longValue)) {
					parsed [groupName] = longValue;
				} else if (double.TryParse(stringValue, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out floatValue)) {
					parsed [groupName] = floatValue;
				} else {
					parsed [groupName] = stringValue;
				}
			}

			return successful.TryProcess(parsed);
		}
	}
}

