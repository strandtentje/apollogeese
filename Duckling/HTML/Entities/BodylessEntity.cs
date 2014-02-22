using System;
using BorrehSoft.ApolloGeese.Duckling.HTML;

namespace BorrehSoft.ApolloGeese.Duckling.Html.Entities
{
	/// <summary>
	/// An HTML entity with only a body and an optional terminating slash
	/// in the single tag it consistst of.
	/// </summary>
	public class BodylessEntity : HtmlEntity
	{
		/// <summary>
		/// Indicates whether this entity will render with or without
		/// a terminating slash. Defaults to false.
		/// </summary>
		public bool HasTerminatingSlash;
		/// <summary>
		/// The .Net-string-formatting Tag Template. Defaults to something useful
		/// so you don't have to modify this.
		/// </summary>
		public string TagTemplate;

		public BodylessEntity (bool HasTerminatingSlash = false, string TagTemplate = "<{0}{1}{2}>") : base("")
		{
			this.HasTerminatingSlash = HasTerminatingSlash;
			this.TagTemplate = TagTemplate;
		}

		public override void WriteUsingCallback (FormattedWriter WriteMethod)
		{
			WriteMethod(TagTemplate,
			            Name,
			            Attributes.ToString(),
			            (HasTerminatingSlash ? " /" : ""));

		}
	}
}
