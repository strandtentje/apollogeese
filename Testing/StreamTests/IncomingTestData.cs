using System;
using System.IO;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections.Maps;

namespace Testing
{
	class IncomingTestData : SimpleIncomingInteraction, IDisposable
	{
		public IncomingTestData (string ingoingDataFile, IInteraction testContext, string sourceName, string mimeType) : base(
			File.OpenRead(ingoingDataFile), testContext, sourceName, mimeType)
		{

		}

		public void Dispose() {
			this.IncomingBody.Close ();
		}
	}
}

