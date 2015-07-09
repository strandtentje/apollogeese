using System;
using System.IO;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace Testing
{
	class IncomingTestData : QuickIncomingInteraction, IDisposable
	{
		public IncomingTestData (string ingoingDataFile, IInteraction testContext) : base(File.OpenRead(ingoingDataFile, testContext)
		{

		}

		public void Dispose() {
			this.IncomingBody.Close ();
		}
	}
}

