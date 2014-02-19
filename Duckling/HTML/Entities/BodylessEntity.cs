using System;
using BorrehSoft.ApolloGeese.Duckling.HTML;

namespace BorrehSoft.ApolloGeese.Duckling.Html.Entities
{
	public class BodylessEntity : HtmlEntity
	{
		public override string Body {
			get {
				return "";
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance has aterminating slash.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance has terminating slash; otherwise, <c>false</c>.
		/// </value>
		public bool HasTerminatingSlash { get; set; }

		public override void WriteUsingCallback (FormattedWriter WriteMethod)
		{
			WriteMethod("<{0}{1}{2}>",
			            Name,
			            Attributes.ToString(),
			            (HasTerminatingSlash ? " /" : ""));

		}
	}
}
