using System;
using BorrehSoft.ApolloGeese.Duckling.HTML;

namespace BorrehSoft.ApolloGeese.Duckling.HTML.Entities
{
	/// <summary>
	/// HTML entity with only a body and no surrounding tags
	/// </summary>
	public class TextualEntity : HtmlEntity
	{		
		/// <summary>
		/// Gets or sets the opening tag format.
		/// </summary>
		/// <value>The opening tag.</value>
		public string OpeningTag { get; set; }
		/// <summary>
		/// Gets or sets the closing tag format
		/// </summary>
		/// <value>The closing tag.</value>
		public string ClosingTag { get; set; }

		/// <summary>
		/// Gets or sets the body text.
		/// </summary>
		/// <value>The body.</value>
		public string Body { get; set; }

		/// <summary>
		/// Gets a value indicating whether this instance is bare, so
		/// not enclosed in HTML tags.
		/// </summary>
		/// <value><c>true</c> if this instance is bare; otherwise, <c>false</c>.</value>
		public bool IsBare { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.ApolloGeese.Duckling.HTML.Entities.TextualEntity"/> class,
		/// this will only produce a non-enclosed textual element.
		/// </summary>
		/// <param name="Body">Textual body of this element.</param>
		public TextualEntity (string Body) : base("")
		{
			this.Body = Body;
			this.IsBare = true;
			this.OpeningTag = "";
			this.ClosingTag = "";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.ApolloGeese.Duckling.HTML.Entities.TextualEntity"/> class,
		/// this will produce a tag-enclosed textual element.
		/// </summary>
		/// <param name="Body">Body of this element.</param>
		/// <param name="Name">Tagname of this element.</param>
		/// <param name="Attributes">Attributes to this element.</param>
		public TextualEntity (string Body, string Name, params HtmlAttribute[] Attributes) : base (Name, Attributes)
		{
			this.Body = Body;
			this.OpeningTag = "<{0}{1}>";
			this.ClosingTag = "</{0}>";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.ApolloGeese.Duckling.HTML.Entities.TextualEntity"/> class,
		/// this will produce a custom-tag-enclosed textual element
		/// </summary>
		/// <param name="Body">Body.</param>
		/// <param name="Name">Name.</param>
		/// <param name="OpeningTag">Opening tag.</param>
		/// <param name="ClosingTag">Closing tag.</param>
		/// <param name="Attributes">Attributes.</param>
		public TextualEntity (string Body, string Name, string OpeningTag, string ClosingTag, params HtmlAttribute[] Attributes) : base(Name, Attributes)
		{
			this.Body = Body;
			this.OpeningTag = OpeningTag;
			this.ClosingTag = ClosingTag;
		}

		public override void WriteUsingCallback (FormattedWriter WriteMethod)
		{
			if (IsBare) {
				WriteMethod (Body);
			} else {
				WriteMethod (OpeningTag, Name, Attributes.ToString ());
				WriteMethod (Body);
				WriteMethod (ClosingTag, Name);
			}
		}
	}
}
