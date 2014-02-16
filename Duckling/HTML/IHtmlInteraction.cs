using System;

namespace BorrehSoft.ApolloGeese.Duckling.HTML
{
	public interface IHtmlInteraction : IInteraction
	{
		HtmlEntity Entity { get; set; }
	}
}
