using System;

namespace BorrehSoft.ApolloGeese.Http.Headers
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

