using System;
using BorrehSoft.ApolloGeese.Duckling.Http;

namespace BorrehSoft.ApolloGeese.Duckling.HTML
{
	public interface IHtmlInteraction : IInteraction
	{
		HtmlEntity Entity { get; set; }
	}
}
