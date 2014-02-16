using System;
using BorrehSoft.ApolloGeese.Duckling.HTML;

namespace BorrehSoft.ApolloGeese.Duckling.HTML.Entities
{
	/// <summary>
	/// HTML entity with only a body and no surrounding tags
	/// </summary>
	public class TextualEntity : HtmlEntity
	{
		public TextualEntity () : base("")
		{
			this.Body = "";
		}

		public TextualEntity (string Body) : base("")
		{
			this.Body = Body;
		}

		public override string Body {
			get; set;
		}

		public override void WriteUsingCallback (FormattedWriter WriteMethod)
		{
			WriteMethod(Body);
		}

	}
}

