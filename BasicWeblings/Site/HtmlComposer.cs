using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Settings;
using BorrehSoft.ApolloGeese.Duckling.Http;
using BorrehSoft.ApolloGeese.Duckling.HTML.Entities;

namespace BorrehSoft.Extensions.BasicWeblings
{
	public class HtmlComposer : Service
	{
		static readonly string[] htmlBranchName = new string[] { "html" };

		public override string[] AdvertisedBranches {
			get {
				return htmlBranchName;
			}
		}

		public string Doctype { get; set; }

		public override string Description {
			get {
				return "HtmlComposer; Element that turns HTML from the single branch in to the response stream";
			}
		}

		protected override void Initialize (Settings modSettings)
		{
			Doctype = modSettings.GetString("doctype", "<!DOCTYPE HTML>");
		}

		protected override bool Process (IInteraction parameters)
		{
			TaggedBodyEntity 
				rootEntity = new TaggedBodyEntity("html"),
				headEntity = new TaggedBodyEntity("head"),
				bodyEntity = new TaggedBodyEntity("body");

			rootEntity.Children.Add(headEntity);
			rootEntity.Children.Add(bodyEntity);

			HtmlInteraction branchInteraction = new HtmlInteraction(parameters, headEntity, bodyEntity);

			return RunBranch ("html", branchInteraction);
		}
	}
}

