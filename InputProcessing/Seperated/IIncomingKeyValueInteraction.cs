using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.IO;
using System.Collections.Generic;
using BorrehSoft.Utensils.Log;
using BorrehSoft.Utensils.Collections;

namespace InputProcessing
{
	public interface IIncomingKeyValueInteraction : IIncomingReaderInteraction
	{
		bool ReadName ();

		object ReadValue();

		bool Readable { get; set; }

		string GetName();

		void SetCurrentValue (object value);

		Map<Service> Actions { get; set; }
	}

}

