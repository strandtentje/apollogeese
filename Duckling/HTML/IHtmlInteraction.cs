using System;

namespace BorrehSoft.ApolloGeese.Duckling.HTML
{
	public interface IHtmlInteraction : IInteraction
	{
		string DoctypeOpener { get; }

		HtmlEntity RootEntity { get; set; }
	}
}

