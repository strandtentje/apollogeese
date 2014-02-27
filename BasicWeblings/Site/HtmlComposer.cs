using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.ApolloGeese.Duckling.Http;
using BorrehSoft.ApolloGeese.Duckling.HTML.Entities;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.ApolloGeese.Duckling.Html.Entities.Specialized;

namespace BorrehSoft.Extensions.BasicWeblings
{
	public class HtmlComposer : Service
	{
		private Service Html;
		string Doctype { get; set; }

		public override string Description {
			get {
				return "HtmlComposer; Element that turns HTML from the single branch in to the response stream";
			}
		}

		protected override void Initialize (Settings modSettings)
		{
			Branches ["html"] = Stub;
			Doctype = modSettings.GetString("doctype", "<!DOCTYPE HTML>");
		}

		protected override void HandleItemChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name = "html")
				Html = e.NewValue;
		}

		protected override bool Process (IInteraction parameters)
		{
			bool success;
			TaggedBodyEntity 
				rootEntity = new TaggedBodyEntity ("html"),
				headEntity = new HeadEntity (),
				bodyEntity = new TaggedBodyEntity("body");

			rootEntity.Children.Add(headEntity);
			rootEntity.Children.Add(bodyEntity);

			HtmlInteraction branchInteraction = new HtmlInteraction(parameters, headEntity, bodyEntity);

			success = Html.TryProcess (branchInteraction);

			return success;
		}
	}
}

