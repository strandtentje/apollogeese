using System;
using BorrehSoft.BorrehSoft.Utensils.Collections;
using System.IO;
using System.Collections.Specialized;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BorrehSoft.ApolloGeese.Duckling.Http.Headers
{
	public class MimeTypeMismatchException : Exception
	{
		public MimeTypeMismatchException (MimeType expectedType, MimeType actualType) :
			base(string.Format("Expeted the Mime-type Name-tuple {0} but instead {1} was found.",
			                   expectedType.ToString(),
			                   actualType.ToString()))
		{

		}
	}
}

