using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.ApolloGeese.Http.Headers;

namespace BorrehSoft.ApolloGeese.Extensions.BasicHttpServer
{
	public class HttpStatus : Service
	{
		public override string Description {
			get {
				return "HTTP status code setter";
			}
		}

		[Instruction("HTTP status code to produce", 200)]
		public int Statuscode { get; set; }

		string MimeType {
			get;
			set;
		}

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
			this.Statuscode = modSettings.GetInt ("statuscode", 200);
			this.MimeType = modSettings.GetString ("mime", "");
		}

		protected override bool Process (IInteraction parameters)
		{
			HttpInteraction interaction = (HttpInteraction)parameters.GetClosest (typeof(HttpInteraction));
			interaction.SetStatuscode (this.Statuscode);

			if (MimeType.Length > 0) {
				interaction.ResponseHeaders.ContentType = new MimeType (this.MimeType);
			}

			return nextService.TryProcess(parameters);
		}
	}
}

