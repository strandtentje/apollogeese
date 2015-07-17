using System;

namespace BorrehSoft.ApolloGeese.Duckling
{
	public interface INosyInteraction : IInteraction
	{
		bool IncludeContext { get; }

		string Signature { get; set; }
	}
}

