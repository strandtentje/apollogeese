using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.IO;
using System.Collections.Generic;
using BorrehSoft.Utensils.Log;

namespace InputProcessing
{
	interface IKeyValueReader : IIncomingBodiedInteraction
	{
		bool TryGetName (out string name);

		void FinalizeValidation ();

	}

}

