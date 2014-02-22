using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Settings;
using BorrehSoft.ApolloGeese.Duckling.HTML;
using BorrehSoft.Utensils;
using BorrehSoft.ApolloGeese.Duckling.Http;

namespace BorrehSoft.Extensions.BasicWeblings
{
	/// <summary>
	/// Element that turns an Html Interaction in to the output stream.
	/// </summary>
	public class HtmlInteraction : Map<object>, IHtmlInteraction
	{
		private IInteraction parent;
		private HtmlEntity htmlEntity;

		public IInteraction RootInteraction { 
			get {
				return parent.RootInteraction; 
			}
		}

		public IInteraction ParentInteraction {
			get {
				return parent;
			}
		}

		public HtmlEntity Entity {
			get { return htmlEntity; }
			set { htmlEntity = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Extensions.BasicWeblings.HtmlInteraction"/> class.
		/// </summary>
		/// <param name='Parent'>
		/// Parent.
		/// </param>
		public HtmlInteraction (IInteraction parent, HtmlEntity htmlEntity)
		{
			this.parent = parent;
			this.htmlEntity = htmlEntity;
		}
	}
}

