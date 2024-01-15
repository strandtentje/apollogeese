using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;

namespace Services.Simple
{
	public class Future : SingleBranchService
	{
		public override string Description {
			get {
				return string.Format("create timestamp {0} into the future, into {1}", this.TimeIncrease, this.VariableName);
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			string[] options = defaultParameter.Split('>');
			if (options.Length == 2) {
				string[] suboptions = options[0].Split('|');
				if (suboptions.Length == 2)
				{
					Settings["timespan"] = suboptions[0];
					Settings["format"] = suboptions[1];
                } else
				{
                    Settings["timespan"] = options[0];
                }				
				Settings["variablename"] = options[1];
			} else if (options.Length == 1) 
			{
				Settings["variablename"] = options[0];
			}
		}
         
		string VariableName;
		TimeSpan TimeIncrease;
        private string Format;

        protected override void Initialize (Settings settings)
		{
			this.TimeIncrease = TimeSpan.Parse(settings.GetString("timespan"));
			this.Format = settings.GetString("format", "yyyy-MM-dd HH:mm:ss");
			this.VariableName = settings.GetString("variablename");
		}

		protected override bool Process (IInteraction parameters)
		{
			return WithBranch.TryProcess(
				new SimpleInteraction(
					parameters, 
					this.VariableName, 
					(DateTime.Now + this.TimeIncrease).ToString(Format)));
		}
	}
}

