using System;
using BorrehSoft.ApolloGeese.Duckling.HTML;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Duckling.HTML.Entities
{
	/// <summary>
	/// A body enclosed in tags
	/// </summary>
	public class TaggedBodyEntity : HtmlEntity
	{
		/// <summary>
		/// Gets or sets the opening tag format.
		/// </summary>
		/// <value>The opening tag.</value>
		public string OpeningTag { get; set; }
		/// <summary>
		/// Gets or sets the closing tag format
		/// </summary>
		/// <value>The closing tag.</value>
		public string ClosingTag { get; set; }

		/// <summary>
		/// The default children, for when no children are supplied when writing using a callback.
		/// </summary>
		public List<HtmlEntity> Children = new List<HtmlEntity>();

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.ApolloGeese.Duckling.HTML.Entities.TaggedBodyEntity"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="attributes">Attributes.</param>
		public TaggedBodyEntity(string name, params HtmlAttribute[] attributes) 
			: base(name, attributes)
		{
			this.OpeningTag = "<{0}{1}>";
			this.ClosingTag = "</{0}>";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.ApolloGeese.Duckling.HTML.Entities.TaggedBodyEntity"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="OpeningTag">Opening tag.</param>
		/// <param name="ClosingTag">Closing tag.</param>
		/// <param name="attributes">Attributes.</param>
		public TaggedBodyEntity(string name, string OpeningTag, string ClosingTag, params HtmlAttribute[] attributes) : base(name, attributes)
		{
			this.OpeningTag = OpeningTag;
			this.ClosingTag = ClosingTag;
		}

		/// <summary>
		/// Passes this element and its default children as a string to the supplied callback.
		/// </summary>
		/// <param name="WriteMethod">Write method.</param>
		public override void WriteUsingCallback (FormattedWriter WriteMethod)
		{
			WriteUsingCallback (WriteMethod, this.Children);
		}

		/// <summary>
		/// Passes this element and the specified children as a string to the supplied callback.
		/// Carefully re-evaluate your life decisions if you end up using this.
		/// </summary>
		/// <param name="WriteMethod">Write method.</param>
		/// <param name="Children">Children.</param>
		public void WriteUsingCallback (FormattedWriter WriteMethod, IEnumerable<HtmlEntity> Children)
		{
			OpenUsingCallback(WriteMethod);

			foreach(HtmlEntity entity in Children)
				entity.WriteUsingCallback(WriteMethod);


			CloseUsingCallback(WriteMethod);
		}

		public void OpenUsingCallback(FormattedWriter WriteMethod) 
		{			
			WriteMethod(OpeningTag, Name,
			            Attributes.ToString());
		}

		public void CloseUsingCallback(FormattedWriter WriteMethod)
		{			
			WriteMethod(ClosingTag, Name);
		}
	}
}

