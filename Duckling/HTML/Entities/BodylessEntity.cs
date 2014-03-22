using System;
using BorrehSoft.ApolloGeese.Duckling.HTML;
using BorrehSoft.Utensils;

namespace BorrehSoft.ApolloGeese.Duckling.HTML.Entities
{
	/// <summary>
	/// An HTML entity with only a body and an optional terminating slash
	/// in the single tag it consistst of.
	/// </summary>
	public class BodylessEntity : HtmlEntity
	{
		public override string TagCloser {
			get {
				return "/>";
			}
		}

		public BodylessEntity (string Name) : base(Name)
		{

		}

		public override void WriteWithDelegate (FormattedWriter WriteMethod)
		{
			WriteMethod(NameOpener, this.Name);
			Attributes.WriteUsingCallback(WriteMethod);
			WriteMethod(TagCloser);
		}
	}
}
