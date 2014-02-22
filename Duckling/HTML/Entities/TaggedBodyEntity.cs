using System;
using BorrehSoft.ApolloGeese.Duckling.HTML;
using BorrehSoft.Utensils;

namespace BorrehSoft.ApolloGeese.Duckling.HTML.Entities
{
	public class TaggedBodyEntity : HtmlEntity
	{
		public TaggedBodyEntity(string name, params HtmlAttribute[] attributes) : base(name, attributes)
		{}

		public List<HtmlEntity> Children = new List<HtmlEntity>();

		public override void WriteUsingCallback (FormattedWriter WriteMethod)
		{
			WriteMethod("<{0}{1}>",
			            Name,
			            Attributes.ToString());

			foreach(HtmlEntity entity in Children)
				entity.WriteUsingCallback(WriteMethod);


			WriteMethod("</{0}>", Name);
		}
	}
}

