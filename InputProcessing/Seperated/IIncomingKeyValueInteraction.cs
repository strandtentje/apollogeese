using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.IO;
using System.Collections.Generic;
using BorrehSoft.Utensils.Log;

namespace InputProcessing
{
	public interface IIncomingKeyValueInteraction : IIncomingReaderInteraction
	{
		bool ReadNextName ();

		string GetCurrentName();

		bool Finalized { get; set; }

	}

}

