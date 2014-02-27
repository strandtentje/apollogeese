using System;
using BorrehSoft.ApolloGeese.Duckling.Html.Entities.Specialized;

namespace BorrehSoft.ApolloGeese.Duckling.HTML
{
	/// <summary>
	/// Interace to an html interaction.
	/// </summary>
	public interface IHtmlInteraction : IInteraction
	{
		/// <summary>
		/// Gets or sets the head element
		/// </summary>
		/// <value>The head element.</value>
		HeadEntity Head { get; set; }

		/// <summary>
		/// Gets or sets the layout element.
		/// </summary>
		/// <value>The layout element.</value>
		HtmlEntity Layout { get; set; }
	}
}
