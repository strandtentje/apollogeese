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
		bool ReadName ();

		object ReadValue();

		string GetName();

		void SetCurrentValue (object value);

		void SetCurrentAction (Service view);

		Service GetAction (string actionName);
	}

}

