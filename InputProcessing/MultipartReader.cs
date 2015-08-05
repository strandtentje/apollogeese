using System;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.ApolloGeese.Http;
using System.IO;
using System.Net;

namespace BorrehSoft.ApolloGeese.Extensions.InputProcessing
{
	public class MultipartReader : FieldReader
	{
		string GetBoundary (IHttpInteraction httpInteraction)
		{
			string contentType = httpInteraction.RequestHeaders.Backend.Get ("Content-Type");
			string prefix = "multipart/form-data; ";
			string boundary;

			if (contentType.StartsWith (prefix)) {
				string suffix = contentType.Substring (prefix.Length).Trim ();

				string boundaryOpener = "boundary=";

				if (suffix.StartsWith (boundaryOpener)) {
					boundary = suffix.Substring (boundaryOpener.Length);
				} else {
					throw new Exception ("missing boundary");
				}
			} else {
				throw new Exception ("missing content type");
			}

			return boundary;
		}

		public override Map<object> GetParseableInput (IInteraction parameters)
		{
			Map<object> parsed = new Map<object> ();

			IInteraction httpCandidate;
			if (parameters.TryGetClosest (typeof(IHttpInteraction), out httpCandidate)) {
				IHttpInteraction httpInteraction = (IHttpInteraction)httpCandidate;

				string boundary = GetBoundary (httpInteraction);

				StreamReader reader = httpInteraction.GetIncomingBodyReader ();

				for (string foundBoundary = reader.ReadLine(); 
					foundBoundary.CompareTo (boundary); 
					foundBoundary = reader.ReadLine ()) {

					WebHeaderCollection headers = new WebHeaderCollection ();

					for (string foundHeader = reader.ReadLine ();
						foundHeader.Length > 0;
						foundHeader = reader.ReadLine ()) {
						headers.Add (foundHeader);
					}

					headers.Get("Content-Disposition")
				}



			} else {
				throw new Exception ("missing http context");
			}

			return parsed;
		}

	}
}

