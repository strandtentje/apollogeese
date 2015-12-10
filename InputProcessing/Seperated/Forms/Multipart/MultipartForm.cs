using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.ApolloGeese.Http;

namespace InputProcessing
{
	public class MultipartForm : Form
	{
		private static string FindBoundary(string contentTypeString) {

			string[] contentType = contentTypeString.Split (';');

			string boundary = "";

			if (contentType [0] == "multipart/form-data") {
				string[] boundarySpec = contentType [1].TrimStart ().Split ('=');

				if (boundarySpec [0] == "boundary") {
					if (boundarySpec [1].StartsWith ("--")) {
						boundary = boundarySpec [1];
					} else {
						throw new FormException ("Boundary short start with a bunch of dashes!");
					}
				} else {
					throw new FormException ("Expected boundary specification after content type");
				}
			} else {
				throw new FormException ("Bad content type; expected multipart/form-data");
			}

			return boundary;
		}

		protected override IRawInputInteraction GetReader (IInteraction parameters)
		{
			IInteraction ancestor;
			IHttpInteraction httpAncestor;
			IRawInputInteraction inputReader;

			if (parameters.TryGetClosest (typeof(IHttpInteraction), out ancestor)) {
				httpAncestor = (IHttpInteraction)ancestor;

				string boundary = FindBoundary (httpAncestor.RequestMethod ["Content-Type"]);

				inputReader = new MultipartKeyValueInteraction (
					parameters, GetTextReader (parameters),	boundary, this.FieldOrder);


			} else {
				throw new Exception ("Multipart form only works in http context.");
			}

			return inputReader;
		}
	}
}

