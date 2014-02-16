using System;
using BorrehSoft.ApolloGeese.Duckling.HTML;

namespace BorrehSoft.ApolloGeese.Duckling
{
	/// <summary>
	/// Html Entity without a body.
	/// </summary>
	public class UnbodiedEntity : HtmlEntity
	{
		/// <summary>
		/// Gets or sets a value indicating whether this instance has a terminating slash.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance has terminating slash; otherwise, <c>false</c>.
		/// </value>
		public bool HasTerminatingSlash { get; set; }

		public override string Body {
			get {
				return "";
			}
		}

		public override void WriteUsingCallback (FormattedWriter WriteMethod)
		{
			WriteMethod("<{0}{1}{2}>",
			            Name,
			            Attributes.ToString(),
			            (HasTerminatingSlash ? " /" : ""));
		}
	}
}

