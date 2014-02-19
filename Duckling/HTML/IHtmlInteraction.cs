using System;

namespace BorrehSoft.ApolloGeese.Duckling.HTML
{
	public interface IHtmlInteraction : IHttpInteraction
	{
		HtmlEntity Entity { get; set; }
	}
}
