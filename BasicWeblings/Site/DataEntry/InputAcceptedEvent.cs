using System;
using BorrehSoft.ApolloGeese.Duckling.Http;
using System.IO;
using BorrehSoft.Utensils;
using BorrehSoft.ApolloGeese.Duckling;
using System.Text;

namespace BorrehSoft.Extensions.BasicWeblings
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

		/// <summary>
		/// Gets the values.
		/// </summary>
		/// <value>The values.</value>
		public Map<string> Values { get; private set; }

		public InputAcceptedEventArgs (IInteraction Parameters, Map<string> Values)
		{
			this.Parameters = Parameters;
			this.Values = Values;
		}
	}
}

