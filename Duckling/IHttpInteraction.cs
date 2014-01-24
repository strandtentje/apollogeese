using System;
using BorrehSoft.Utensils;
using System.IO;

namespace BorrehSoft.ApolloGeese.Duckling
{
	/// <summary>
	/// Parameters to an HTTP-related service-interaction.
	/// </summary>
	public interface IHttpInteraction : IInteraction
	{
		/// <summary>
		/// Gets the URL chunk-list
		/// </summary>
		/// <value>
		/// The URL
		/// </value>
		StringList URL { get; }

		/// <summary>
		/// Gets or sets the title of the current block
		/// </summary>
		/// <value>
		/// The current title.
		/// </value>
		string CurrentTitle { get; set; }

		/// <summary>
		/// Appends text to body.
		/// </summary>
		/// <param name='copy'>
		/// Copy to append
		/// </param>
		/// <param name='type'>
		/// Type of copy (MIME!) (i.e. text/html)
		/// </param>
		void AppendToBody (string copy, string type);

		/// <summary>
		/// Sets the body.
		/// </summary>
		/// <param name='sourceStream'>
		/// Source stream.
		/// </param>
		/// <param name='sourceType'>
		/// Source type (MIME!)
		/// </param>
		/// <param name='sourceLength'>
		/// Source length.
		/// </param>
		void SetBody (Stream sourceStream, string sourceType, long sourceLength);

		/// <summary>
		/// Gets or sets the status code for the HTTP response
		/// </summary>
		/// <value>
		/// The status code.
		/// </value>
		int StatusCode { get; set; }

	}
}

