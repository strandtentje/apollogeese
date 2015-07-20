using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Extensions.BasicHttpServer
{
	public class HttpStatus : Service
	{
		public override string Description {
			get {
				return "HTTP status code setter";
			}
		}

		private int Statuscode { get; set; }

		private Service nextService;

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "next") {
				nextService = e.NewValue;
			}
		}

		public override void LoadDefaultParameters (object defaultParameter)
		{
			this.Settings ["statuscode"] = defaultParameter;
		}

		protected override void Initialize (Settings modSettings)
		{
			this.Statuscode = modSettings.GetInt ("statuscode");
		}

		protected override bool Process (IInteraction parameters)
		{
			HttpInteraction interaction = (HttpInteraction)parameters.GetClosest (typeof(HttpInteraction));
			interaction.SetStatuscode (this.Statuscode);

			return nextService.TryProcess(parameters);
		}
	}
}

