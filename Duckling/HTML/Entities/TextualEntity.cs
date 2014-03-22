using System;
using BorrehSoft.ApolloGeese.Duckling.HTML;
using BorrehSoft.Utensils;

namespace BorrehSoft.ApolloGeese.Duckling.HTML.Entities
{
	/// <summary>
	/// HTML entity with only a body and no surrounding tags
	/// </summary>
	public class TextualEntity : HtmlEntity
	{		
		/// <summary>
		/// Gets or sets the body text.
		/// </summary>
		/// <value>The body.</value>
		public string Body { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.ApolloGeese.Duckling.HTML.Entities.TextualEntity"/> class,
		/// this will only produce a non-enclosed textual element.
		/// </summary>
		/// <param name="Body">Textual body of this element.</param>
		public TextualEntity (string Name = "", string Body = "") : base(Name)
		{
			this.Body = Body;
		}

		public override void WriteWithDelegate (FormattedWriter WriteMethod)
		{
			if (Name.Length == 0) {
				WriteMethod (Body);
			} else {
				WriteMethod (NameOpener, Name);
				Attributes.WriteUsingCallback(WriteMethod);
				WriteMethod (TagCloser);

				WriteMethod (Body);

				WriteMethod (NameCloser, Name);
				WriteMethod (TagCloser);
			}
		}
	}
}
