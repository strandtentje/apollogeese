using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Extensions.Navigation
{
	/// <summary>
	/// Site subsection based on URL.
	/// </summary>
	public class SiteSubsection : Service
	{
		public override string Description {
			get {
				return string.Join(", ", Branches.Dictionary.Keys);
			}
		}

		private const string ResourceNameKeySetting = "resourcenamekey";

		[Instruction("Variable name at which the name of the current resource gets stored")]
		public string ResourceNameKey { get; private set; }

		public override void LoadDefaultParameters (string defaultParameter)
		{
			this.Settings [ResourceNameKeySetting] = defaultParameter;
		}

		protected override void Initialize (Settings modSettings)
		{
			this.ResourceNameKey = modSettings.GetString (
				ResourceNameKeySetting, "resourcename");
			Branches ["main"] = Stub;
			Branches ["default"] = Stub;
		}

		protected override bool Process (IInteraction uncastParameters)
		{
			IHttpInteraction httpParameters = (IHttpInteraction)uncastParameters.GetClosest(
				typeof(IHttpInteraction));

			SubsectionInteraction interaction = new SubsectionInteraction (
				httpParameters, uncastParameters, this.ResourceNameKey);

			Service branch = Stub;

			if (branch == Stub) 
				branch = Branches [interaction.DirectoryName] ?? Stub;
			
			if (branch == Stub)
				branch = Branches ["default"];
			else
				interaction.Confirm ();

			return branch.TryProcess (interaction);
		}
	}
}

