using System;
using System.IO;
using BorrehSoft.Utensils;

namespace BorrehSoft.ApolloGeese.Duckling.HTML
{
	/// <summary>
	/// Html entity.
	/// </summary>
	public abstract class HtmlEntity
	{
		/// <summary>
		/// Gets or sets the opening tag format.
		/// </summary>
		/// <value>The opening tag.</value>
		public virtual string NameOpener { get { return "<{0}"; } }
		/// <summary>
		/// Gets or sets the closing tag format
		/// </summary>
		/// <value>The closing tag.</value>
		public virtual string NameCloser { get { return "</{0}"; } }
		/// <summary>
		/// Gets or sets the tag closer.
		/// </summary>
		/// <value>
		/// The tag closer.
		/// </value>
		public virtual string TagCloser { get { return ">"; } }

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; private set; }

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
		public HtmlEntity (string Name)
		{
			this.Name = Name;
			this.Attributes = new HtmlAttributeCollection();
		}

		/// <summary>
		/// Passes this element as a string to the supplied callback.
		/// </summary>
		/// <param name='WriteMethod'>
		/// Write method.
		/// </param>
		public abstract void WriteWithDelegate (FormattedWriter WriteMethod);
	}
}
