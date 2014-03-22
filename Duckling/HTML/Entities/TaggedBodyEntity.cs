using System;
using BorrehSoft.ApolloGeese.Duckling.HTML;
using System.Collections.Generic;
using BorrehSoft.Utensils;

namespace BorrehSoft.ApolloGeese.Duckling.HTML.Entities
{
	/// <summary>
	/// A body enclosed in tags
	/// </summary>
	public class TaggedBodyEntity : HtmlEntity
	{
		/// <summary>
		/// The default children, for when no children are supplied when writing using a callback.
		/// </summary>
		public List<HtmlEntity> Children = new List<HtmlEntity>();

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.ApolloGeese.Duckling.HTML.Entities.TaggedBodyEntity"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="attributes">Attributes.</param>
		public TaggedBodyEntity(string name) : base(name) { }

		/// <summary>
		/// Passes this element and its default children as a string to the supplied callback.
		/// </summary>
		/// <param name="WriteMethod">Write method.</param>
		public override void WriteWithDelegate (FormattedWriter WriteMethod)
		{
			WriteWithDelegate (WriteMethod, this.Children);
		}

		/// <summary>
		/// Passes this element and the specified children as a string to the supplied callback.
		/// Carefully re-evaluate your life decisions if you end up using this.
		/// </summary>
		/// <param name="WriteMethod">Write method.</param>
		/// <param name="Children">Children.</param>
		public void WriteWithDelegate (FormattedWriter WriteMethod, IEnumerable<HtmlEntity> Children)
		{
			OpenWithDelegate(WriteMethod);

			foreach(HtmlEntity entity in Children)
				entity.WriteWithDelegate(WriteMethod);

			CloseWithDelegate(WriteMethod);
		}

		public void OpenWithDelegate (FormattedWriter WriteMethod)
		{
			WriteMethod(NameOpener, this.Name);
			Attributes.WriteUsingCallback(WriteMethod);
			WriteMethod(TagCloser);
		}

		public void CloseWithDelegate (FormattedWriter WriteMethod)
		{
			WriteMethod(NameCloser, this.Name);
			WriteMethod(TagCloser);
		}
	}
}

