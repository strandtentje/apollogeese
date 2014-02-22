using System;
using BorrehSoft.Utensils;
using System.IO;

namespace BorrehSoft.ApolloGeese.Duckling.HTML
{
	/// <summary>
	/// Html entity.
	/// </summary>
	public abstract class HtmlEntity
	{
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
		public HtmlEntity (string Name, params HtmlAttribute[] Attributes)
		{
			this.Name = Name;
			this.Attributes = new HtmlAttributeCollection(Attributes);
		}

		public delegate void FormattedWriter(string format, params string[] parameters);

		/// <summary>
		/// Passes this element as a string to the supplied callback.
		/// </summary>
		/// <param name='WriteMethod'>
		/// Write method.
		/// </param>
		public abstract void WriteUsingCallback (FormattedWriter WriteMethod);
	}
}
