using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.ApolloGeese.Duckling.HTML;
using BorrehSoft.ApolloGeese.Duckling.Http;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.ApolloGeese.Duckling.HTML.Entities.Specialized;

namespace BorrehSoft.Extensions.BasicWeblings
{
	/// <summary>
	/// Element that turns an Html Interaction in to the output stream.
	/// </summary>
	public class HtmlInteraction : Map<object>, IHtmlInteraction
	{
		private IInteraction parent;

		public HtmlInteraction(IHtmlInteraction parent, HtmlEntity Layout)
		{
			this.parent = parent;
			this.Head = parent.Head;
			this.Layout = Layout;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Extensions.BasicWeblings.HtmlInteraction"/> class.
		/// </summary>
		/// <param name='Parent'>
		/// Parent.
		/// </param>
		public HtmlInteraction (IInteraction parent, HeadEntity Head, HtmlEntity Layout)
		{
			this.parent = parent;
			this.Head = Head;
			this.Layout = Layout;
		}

		public IInteraction Root { 
			get {
				return parent.Root; 
			}
		}

		public IInteraction Parent {
			get {
				return parent;
			}
		}

		public HeadEntity Head { get; set; }

		public HtmlEntity Layout { get; set; }
	}
}

