using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using System.IO;

namespace InputProcessing
{
	public class UrlForm : Form
	{
		protected override IIncomingKeyValueInteraction GetReader (IInteraction parameters)
		{
			return new UrlKeyValueInteraction (parameters, GetTextReader(parameters));

		}
	}
}

