using System;
using BorrehSoft.ApolloGeese.Duckling.Http;
using System.IO;
using BorrehSoft.Utensils;
using BorrehSoft.ApolloGeese.Duckling;
using System.Text;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Page.DataEntry
{
	/// <summary>
	/// Event handler for incorrect data entry
	/// or form presentation
	/// </summary>
	public delegate void FormDisplayingEventHandler(object sender, FormDisplayingEventArgs e);

	/// <summary>
	/// The event arguments to the event that occurs 
	/// on bad data entry or just a bare form.
	/// </summary>
	public class FormDisplayingEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the method that writes markup to the
		/// client
		/// </summary>
		/// <value>The write.</value>
		public StreamWriter Writer { get;	private set; }

		/// <summary>
		/// Gets or sets a value indicating whether
		/// entry was attempted.
		/// </summary>
		/// <value><c>true</c> if entry attempt; otherwise, <c>false</c>.</value>
		public bool EntryAttempt { get; private set; }

		/// <summary>
		/// Gets the values.
		/// </summary>
		/// <value>The values.</value>
		public Map<string> Values { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Extensions.BasicWeblings.FormDisplayingEventArgs"/> class.
		/// </summary>
		/// <param name="Write">Write.</param>
		public FormDisplayingEventArgs (StreamWriter Writer, bool EntryAttempt, Map<string> Values)
		{
			this.Writer = Writer;
			this.EntryAttempt = EntryAttempt;
			this.Values = Values;
		}
	}
}

