using System;
using System.Web;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Settings;
using BorrehSoft.Utensils;

namespace Datatables
{
	/// <summary>
	/// Form interaction.
	/// </summary>
	class WebformInteraction : Map<object>, IInteraction
	{
		IHttpInteraction baseInteraction;

		/// <summary>
		/// Initializes a new instance of the <see cref="Datatables.FormInteraction"/> class.
		/// </summary>
		/// <param name="iHttpInteraction">HTTP-Interaction to base on</param>
		/// <param name="method">Method.</param>
		public WebformInteraction (IHttpInteraction iHttpInteraction, string method)
		{
			baseInteraction = iHttpInteraction;

			if (method.ToLower () == "post")
				ProcessPost ();
			if (method.ToLower () == "get")
				ProcessGet ();
		}

		/// <summary>
		/// Processes a POST-request.
		/// </summary>
		void ProcessPost ()
		{
			if (baseInteraction.RequestMethod.ToLower () != "post")
				return;

			if (baseInteraction.RequestBodyMIME.ToLower () != "application/x-www-form-urlencoded")
				return;

			byte[] data = new byte[baseInteraction.RequestBodySize];
			baseInteraction.RequestBodyStream.Read (data, 0, data.Length);

			this.AddFromString (
				baseInteraction.RequestBodyEncoding.GetString (data),
				HttpUtility.UrlDecode, '=', '&');
		}

		/// <summary>
		/// Processes a GET-request.
		/// </summary>
		void ProcessGet ()
		{
			if (baseInteraction.RequestMethod.ToLower () != "get")
				return;

			this.AddFromString (
				baseInteraction.URL.ReadToEnd (),
				HttpUtility.UrlDecode, '=', '&');
		}
	}
}

