using System;

namespace BorrehSoft.ApolloGeese.Duckling
{
	public interface INosyInteraction : IInteraction
	{
		/// <summary>
		/// ???
		/// </summary>
		/// <value><c>true</c> if include context; otherwise, <c>false</c>.</value>
		bool IncludeContext { get; }

		string Signature { get; set; }
	}
}

