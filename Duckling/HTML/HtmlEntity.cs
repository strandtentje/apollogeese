using System;
using BorrehSoft.Utensils;

namespace BorrehSoft.ApolloGeese.Duckling.HTML
{
	/// <summary>
	/// Html entity.
	/// </summary>
	public class HtmlEntity
	{
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; private set; }

		/// <summary>
		/// Gets or sets the type of encapsulation for this HTML entity.
		/// </summary>
		/// <value>The type of the capsule.</value>
		public HtmlCapsuleType CapsuleType { get; set; }

		/// <summary>
		/// Gets or sets the attributes.
		/// </summary>
		/// <value>The attributes.</value>
		public HtmlAttributeCollection Attributes { get ; set ; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.ApolloGeese.Duckling.HtmlEntity"/> class.
		/// </summary>
		/// <param name="Name">Name.</param>
		/// <param name="HasOpeningTag">If set to <c>true</c> has opening tag.</param>
		/// <param name="HasClosingTag">If set to <c>true</c> has closing tag.</param>
		/// <param name="Attributes">Attributes.</param>
		public HtmlEntity (string Name, HtmlCapsuleType CapsuleType, params Tuple<string, string> Attributes)
		{
			this.Name = Name;
			this.CapsuleType = CapsuleType;
			this.Attributes = new List<System.Tuple<string, string>> (Attributes);
		}

		public override string ToString ()
		{
			return ToString ("");
		}

		public string ToString (string inner)
		{
			switch (CapsuleType) {
			case HtmlCapsuleType.Bare:
				return inner;
				break;
			case HtmlCapsuleType.Capsule:
				return string.Format ("<{0}{1}>{2}</{0}>",
				                      Name,
				                      Attributes.ToString (),
				                      inner);
				break;
			case HtmlCapsuleType.ClosingOpener:
				return string.Format ("<{0}{1}/>",
				                      Name,
				                      Attributes.ToString ());
				break;
			case HtmlCapsuleType.OpenerOnly:
				return string.Format ("<{0}{1}>",
				                      Name,
				                      Attributes.ToString ());
				break;
			default:
				return "";
				break;
			}
		}
	}
}
