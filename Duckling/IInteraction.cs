using System;
using System.Web;
using System.Net;
using System.Collections.Generic;
using BorrehSoft.Utensils.Settings;
using BorrehSoft.Utensils;
using System.Text;
using System.IO;
using System.Collections.Specialized;
using System.Security;
using System.Security.Cryptography;
using GList = System.Collections.Generic.List<string>;

namespace BorrehSoft.ApolloGeese.Duckling
{
	/// <summary>
	/// Parameters to a service call
	/// </summary>
	public interface IInteraction 
	{
		/// <summary>
		/// Gets the root interaction, typically the one that invoked the chain.
		/// This usually, in case of a website, ends you up with an IHttpInteraction.
		/// </summary>
		/// <value>
		/// The root interaction.
		/// </value>
		IInteraction RootInteraction { get; }

		/// <summary>
		/// Gets the parent interaction.
		/// </summary>
		/// <value>
		/// The parent interaction.
		/// </value>
		IInteraction ParentInteraction { get; }

		/// <summary>
		/// Tries to get a string from the luggage
		/// </summary>
		/// <returns>
		/// True if acquisition of string has succeeded
		/// </returns>
		/// <param name='id'>
		/// The name of the string in the luggage
		/// </param>
		/// <param name='luggage'>
		/// The variable into which the string will be loaded
		/// </param>
		bool TryGetString(string id, out string luggage);

		/// <summary>
		/// Tries to get an object from the luggage
		/// </summary>
		/// <returns>
		/// Ttrue if acquisition of object has succeeded
		/// </returns>
		/// <param name='id'>
		/// The name of the object in the luggage
		/// </param>
		/// <param name='luggage'>
		/// The variable into which the object will be loaded
		/// </param>
		bool TryGetValue(string id, out object luggage);

		/// <summary>
		/// Gets or sets the value with the specified name.
		/// </summary>
		/// <param name='name'>
		/// Name.
		/// </param>
		object this [string name] { get; set; }
	}
}

