using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections;
using System.Collections.Generic;
using System.Reflection;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Log;
using BorrehSoft.Utilities.Collections.Settings;
using System.Text.RegularExpressions;
using BorrehSoft.Utilities;
using BorrehSoft.ApolloGeese.Loader;

namespace BorrehSoft.ApolloGeese
{
	class CommandLineArgumentException : Exception
	{
		public CommandLineArgumentException (string helpRequested) : base(helpRequested)
		{
		}
	}

}
