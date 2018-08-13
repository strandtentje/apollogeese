using NUnit.Framework;
using System;
using Auth;

namespace AuthIntergration
{
	[TestFixture ()]
	public class BasicAuthTest
	{
		BasicAuthentication SUT;

		[SetUp()]
		public void SetUp()
		{
			SUT = new BasicAuthentication();
			//SUT.
			
		}

		[Test ()]
		public void TestCase ()
		{
		}
	}
}

