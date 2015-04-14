using System;

namespace BorrehSoft.ApolloGeese.Duckling
{
	/// <summary>
	/// Parameters to a service call
	/// </summary>
	public interface IInteraction : IFast
	{
		/// <summary>
		/// Gets the root interaction, typically the one that invoked the chain.
		/// This usually, in case of a website, ends you up with an IHttpInteraction.
		/// </summary>
		/// <value>
		/// The root interaction.
		/// </value>
		IInteraction Root { get; }

		/// <summary>
		/// Gets the parent interaction.
		/// </summary>
		/// <value>
		/// The parent interaction.
		/// </value>
		IInteraction Parent { get; }

		/// <summary>
		/// Gets the closest ancestor with the specified type.
		/// </summary>
		/// <returns>The closest ancestor with the specified type.</returns>
		/// <param name="t">The specified type.</param>
		IInteraction GetClosest(Type t);

		/// <summary>
		/// Tries to get the nearest ancestor with the specified type.
		/// </summary>
		/// <returns><c>true</c>, if the ancestor with the specified type was found.</returns>
		/// <param name="t">Type to look for</param>
		/// <param name="closest">Found ancestor</param>
		bool TryGetClosest (Type t, out IInteraction closest);

		/// <summary>
		/// Clone this interaction, and give it a new parent. Don't
		/// tell the biologists I can do this.
		/// </summary>
		/// <param name="parent">Parent.</param>
		IInteraction Clone(IInteraction parent);

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

		/// <summary>
		/// Same as TryGetFallback, but only succeeds if string is found.
		/// </summary>
		/// <returns><c>true</c>, if get fallback string was tryed, <c>false</c> otherwise.</returns>
		/// <param name="id">Identifier.</param>
		/// <param name="luggage">Luggage.</param>
		bool TryGetFallbackString(string id, out string luggage);

		/// <summary>
		/// Scans from here to ancestors for data at specified name. Returns false if nothing found.
		/// </summary>
		/// <returns><c>true</c>, if get fallback was tryed, <c>false</c> otherwise.</returns>
		/// <param name="id">Identifier.</param>
		/// <param name="luggage">Luggage.</param>
		bool TryGetFallback (string id, out object luggage);
	}
}