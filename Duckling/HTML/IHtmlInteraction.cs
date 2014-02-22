using System;

namespace BorrehSoft.ApolloGeese.Duckling.HTML
{
	public interface IHtmlInteraction : IInteraction
	{
		IHtmlInteraction BaseHtmlInteraction { get; }

		HtmlEntity BodyEntity { get; set; }

		HtmlEntity HeadEntity { get; set; }
	}
}
