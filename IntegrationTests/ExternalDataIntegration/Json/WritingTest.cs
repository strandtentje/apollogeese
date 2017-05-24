using System;
using NUnit.Framework;
using BorrehSoft.Utilities.Log;
using BorrehSoft.ApolloGeese.Loader;
using System.IO;
using BorrehSoft.ApolloGeese.CoreTypes;

namespace Json
{
	[TestFixture()]
	public class WritingTest
	{
		SimpleInteraction BaseInteraction;

		ServiceCollection Services {
			get;
			set;
		}

		[SetUp ()]
		public void SetUp ()
		{
			(new Secretary ("integration")).ReportHere (5, "open!");

			Services = ServiceCollectionCache.Get (
				Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Json", "readingtest.conf"), 
				Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Json"),
				false, true
			);

			BaseInteraction = new SimpleInteraction ();
		}

		[TearDown ()]
		public void TearDown ()
		{
			Secretary.LatestLog.Dispose ();
		}

		[Test ()]
		public void EmptyTester ()
		{
			
		}

	}
}

