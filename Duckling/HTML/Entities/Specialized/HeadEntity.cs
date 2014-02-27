using System;
using BorrehSoft.ApolloGeese.Duckling.HTML.Entities;

namespace BorrehSoft.ApolloGeese.Duckling.Html.Entities.Specialized
{
	/// <summary>
	/// HTML Head entity.
	/// </summary>
	public class HeadEntity : TaggedBodyEntity
	{
		private TextualEntity title;

		public HeadEntity () : base("head")
		{

		}

		/// <summary>
		/// Gets or sets the title of the document.
		/// </summary>
		/// <value>The title.</value>
		public string Title {
			get {
				if (title == null)
					return "";
				return title.Body;
			} set {
				if (title = null) {
					title = new TextualEntity (value, "title");
					this.Children.Add (title);
				}
				else 
					title.Body = value;
			}			    
		}
	}
}

