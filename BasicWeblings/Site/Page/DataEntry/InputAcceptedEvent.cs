using System;
using BorrehSoft.ApolloGeese.Duckling.Http;
using System.IO;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.ApolloGeese.Duckling;
using System.Text;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Page.DataEntry
{
	/// <summary>
	/// The event handler for when
	/// data entered in the form was correct.
	/// </summary>
	public delegate void InputAcceptedEventHandler(object sender, InputAcceptedEventArgs e);

	/// <summary>
	/// Event arguments to the event that gets raised when the
	/// data entered in the form was correct.
	/// </summary>
	public class InputAcceptedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the parameters that the form values may
		/// be written to
		/// </summary>
		/// <value>The parameters.</value>
		public IInteraction Parameters { get; set; }

		public InputAcceptedEventArgs (IInteraction Parameters)
		{
			this.Parameters = Parameters;
		}
	}
}

