using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Settings;
using System.Collections.Generic;
using UList = BorrehSoft.Utensils.List<object>;
using BorrehSoft.ApolloGeese.Duckling.Http;
using System.IO;
using BorrehSoft.Utensils;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;

namespace BorrehSoft.Extensions.BasicWeblings
{
	public class Form : Service
	{
		HtmlTag FormTag = new HtmlTag ("form");

		#region Request Eater
		/// <summary>
		/// Gets or sets the method.
		/// </summary>
		/// <value>The method.</value>
		public string Method { get; set; }

		/// <summary>
		/// Tries to get a map of whatever message body there is (POST/GET);
		/// </summary>
		/// <returns><c>true</c>, if get body map was tryed, <c>false</c> otherwise.</returns>
		/// <param name="httpInteraction">Http interaction.</param>
		/// <param name="givenData">Given data.</param>
		bool TryGetBodyMap (IHttpInteraction httpInteraction, out Map<string> givenData)
		{
			int startOfQuery; string body;

			givenData = new Map<string> ();

			if ((Method == "POST") && IsPost (httpInteraction)) {
				body = httpInteraction.RequestBody.ReadToEnd ();
				if (body.Length < 1)
					return false;
			} else if ((Method == "GET") && IsGet (httpInteraction)) {
				body = httpInteraction.URL [httpInteraction.URL.Count - 1];
				startOfQuery = body.IndexOf ('?');
				if (startOfQuery > -1) {
					body = body.Substring (startOfQuery + 1);
				} else {
					return false;
				}
			} else {
				return false;
			}

			givenData.AddFromString(body, HttpUtility.UrlDecode, '=', '&');

			return true;
		}

		/// <summary>
		/// Determines if the specified httpInteraction has a GET request.
		/// </summary>
		/// <returns><c>true</c> if is get the specified httpInteraction; otherwise, <c>false</c>.</returns>
		/// <param name="httpInteraction">Http interaction.</param>
		static bool IsGet (IHttpInteraction httpInteraction)
		{
			return httpInteraction.RequestMethod.ToUpper () == "GET";
		}

		/// <summary>
		/// Determines if the specified httpInteraction has a POST request.
		/// </summary>
		/// <returns><c>true</c> if is post the specified httpInteraction; otherwise, <c>false</c>.</returns>
		/// <param name="httpInteraction">Http interaction.</param>
		static bool IsPost (IHttpInteraction httpInteraction)
		{
			return httpInteraction.RequestMethod.ToUpper () == "POST";
		}
		#endregion

		static string branchRegex = "field[0-9]+";
		static Regex branchMatcher = new Regex (branchRegex);

		public override string[] AdvertisedBranches {
			get { return new string[] { branchRegex, "process" }; }
		}

		public override string Description {
			get {
				return "Form, area with multiple data entry fields.";
			}
		}

		protected override void Initialize (Settings modSettings)
		{
			FormTag.Attributes ["action"] = "";

			object fieldsObject;

			Method = modSettings.GetString ("method", "POST");

			FormTag.Attributes ["method"] = Method.ToLower ();

			FormTag.Rerender ();
		}

		protected override bool Process (IInteraction parameters)
		{
			#region Allocating for this thread...
			IHttpInteraction httpInteraction; 
			EntryInteraction entryInteraction;
			Map<string> data; bool success;
			StringBuilder confirmation;
			Action<string> write;
			#endregion

			data = new Map<string>();
			httpInteraction = parameters as IHttpInteraction;

			// Store POST/GET into data
			success = TryGetBodyMap (httpInteraction, out data);

			// Put the parsed data into an interaction to be used by
			// fields in this form.
			entryInteraction = new EntryInteraction (Values: data);

			// Inform all fields about the new input and check if they're
			// cool with the input.
			foreach (string branch in ConnectedBranches.Keys) 
				if (branchMatcher.IsMatch (branch)) 
					success &= RunBranch (branch, entryInteraction);	

			if (success) {
				// If all fields were cool with the new input, let them push 
				// the input (optionally parsed) into the main interaction, 
				// and branch off to the submission-branch.
				entryInteraction.RaiseInputAccepted (parameters);
				return RunBranch ("process", parameters);
			} else {
				write = httpInteraction.ResponseBody.Write;
				// If not all fields were cool with the new input, let each
				// field write its markup back to the client.
				write (FormTag.Head);
				entryInteraction.RaiseFormDisplaying (
					Writer: httpInteraction.ResponseBody, 
					EntryAttempt: data.Length > 0);
				write (FormTag.Tail);
				return true;
			}
		}
	}
}
