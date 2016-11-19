using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections.Maps;

namespace BorrehSoft.ApolloGeese.Extensions.BasicHttpServer
{
	public class HttpStatus : SingleBranchService
	{
		public override string Description {
			get {
				return "HTTP status code setter";
			}
		}

		[Instruction("HTTP status code to produce", 200)]
		public int Statuscode { get; set; }

		[Instruction("Charset this response is in", "utf-8")]
		string CharSet {
			get;
			set;
		}

		[Instruction("Mimetype of this response", "")]
		string MimeType {
			get;
			set;
		}

		public override void LoadDefaultParameters (object defaultParameter)
		{
			this.Settings ["statuscode"] = defaultParameter;
		}

		protected override void Initialize (Settings modSettings)
		{
			this.Statuscode = modSettings.GetInt ("statuscode", 200);
			this.MimeType = modSettings.GetString ("mimetype", "");
			this.CharSet = modSettings.GetString ("charset", "");
		}

		protected override bool Process (IInteraction parameters)
		{
			HttpInteraction interaction = (HttpInteraction)parameters.GetClosest (typeof(HttpInteraction));

			string mimeType;

			if (MimeType.Length > 0) {
				if (this.CharSet.Length > 0) {
					mimeType = string.Format (
						"{0}; charset={1}",
						this.MimeType, 
						this.CharSet);
				} else {
					mimeType = this.MimeType;
				}
				interaction.SetContentType (mimeType);
			} 

			interaction.SetStatusCode (this.Statuscode);

			return WithBranch.TryProcess(parameters);
		}
	}
}

