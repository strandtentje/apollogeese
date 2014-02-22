using System;
using BorrehSoft.ApolloGeese.Duckling.HTML;
using BorrehSoft.BorrehSoft.Utensils.Collections;

namespace BorrehSoft.ApolloGeese.Duckling.HTML.Entities
{
	public class TaggedBodyEntity : HtmlEntity
	{
		public string 
			OpeningTag = "<{0}{1}>",
			ClosingTag = "</{0}>";

		public TaggedBodyEntity(string name, string OpeningTag, string ClosingTag, params HtmlAttribute[] attributes) : base(name, attributes)
		{}

		public TaggedBodyEntity(string name, params HtmlAttribute[] attributes) : base(name, attributes)
		{}

		public List<HtmlEntity> Children = new List<HtmlEntity>();

		public override void WriteUsingCallback (FormattedWriter WriteMethod)
		{
			WriteMethod(OpeningTag, Name,
			            Attributes.ToString());

			foreach(HtmlEntity entity in Children)
				entity.WriteUsingCallback(WriteMethod);


			WriteMethod(ClosingTag, Name);
		}
	}
}

