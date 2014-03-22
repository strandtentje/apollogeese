using System;
using BorrehSoft.ApolloGeese.Duckling.HTML;

namespace BorrehSoft.ApolloGeese.Duckling.HTML.Entities
{
	/// <summary>
	/// An HTML entity with only a body and an optional terminating slash
	/// in the single tag it consistst of.
	/// </summary>
	public class BodylessEntity : HtmlEntity
	{
		/// <summary>
		/// The .Net-string-formatting Tag Template. Defaults to something useful
		/// so you don't have to modify this.
		/// </summary>
		public string TagTemplate = "<{0}{1}>";

		public BodylessEntity (string Name, params HtmlAttribute[] Attributes) : base(Name, Attributes)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.ApolloGeese.Duckling.Html.Entities.BodylessEntity"/> class.
		/// </summary>
		/// <param name="HasTerminatingSlash">If set to <c>true</c>, has terminating slash.</param>
		/// <param name="TagTemplate">Tag template format.</param>
		public BodylessEntity (string Name, string TagTemplate, params HtmlAttribute[] Attributes) : base(Name, Attributes)
		{
			this.TagTemplate = TagTemplate;
		}

		public override void WriteUsingCallback (FormattedWriter WriteMethod)
		{
			WriteMethod(TagTemplate,
			            Name,
			            Attributes.ToString());

		}
	}
}
