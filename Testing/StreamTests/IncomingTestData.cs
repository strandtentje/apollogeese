using System;
using System.IO;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace Testing
{
	class IncomingTestData : QuickIncomingInteraction, IDisposable
	{
		public IncomingTestData (string ingoingDataFile, IInteraction testContext, string sourceName) : base(
			File.OpenRead(ingoingDataFile), testContext, sourceName)
		{

		}

		public void Dispose() {
			this.IncomingBody.Close ();
		}
	}
}

