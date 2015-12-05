using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.ApolloGeese.Http;

namespace InputProcessing
{
	public class MultipartForm : Form
	{
		protected override IRawInputInteraction GetReader (IInteraction parameters)
		{
			IInteraction ancestor;
			IHttpInteraction httpAncestor;

			if (parameters.TryGetClosest (typeof(IHttpInteraction), out ancestor)) {
				httpAncestor = (IHttpInteraction)ancestor;

				// httpAncestor.
			} else {
				throw new Exception ("Multipart form only works in http context.");
			}
		}
	}
}

