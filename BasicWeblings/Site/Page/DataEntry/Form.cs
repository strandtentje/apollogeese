using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;
using BorrehSoft.ApolloGeese.Duckling.Http;
using System.IO;
using BorrehSoft.Utensils.Collections;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Extensions.BasicWeblings.Server;
using BorrehSoft.ApolloGeese.Duckling.HTML.Entities;
using BorrehSoft.ApolloGeese.Duckling.HTML;
using BorrehSoft.Utensils;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Page.DataEntry
{
	/// <summary>
	/// A form on a webpage displaying connected fields or -when filled correctly- whatever is connected to output
	/// </summary>
	public class Form : Service
	{
		/// <summary>
		/// Gets or sets the method.
		/// </summary>
		/// <value>The method.</value>
		public string Method { get; set; }

		/// <summary>
		///  Gets the description of this service. (Cool bonus: May change! Woo!) May be used as page titles 
		/// </summary>
		/// <value>
		/// The name of this service
		/// </value>
		public override string Description {
			get {
				return "Form, area with multiple data entry fields.";
			}
		}

		/// <summary>
		/// The form tag.
		/// </summary>
		private TaggedBodyEntity FormTag;

		/// <summary>
		/// The inputs.
		/// </summary>
		private List<Service> Inputs = new List<Service>();

		/// <summary>
		/// The output block.
		/// </summary>
		private Service Output = Stub;

		/// <summary>
		/// The branch-name regex-matcher.
		/// </summary>
		static Regex branchMatcher = new Regex ("\\w+_field");

		protected override void Initialize (Settings modSettings)
		{
			Method = modSettings.GetString ("method", "POST");
			
			FormTag = new TaggedBodyEntity("form");

			FormTag.Attributes["action"] = "";
			FormTag.Attributes["method"] = Method;
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "output") {
				Output = e.NewValue;
			} else if (branchMatcher.IsMatch (e.Name)) {
				if (e.PreviousValue != null) {
					if (Inputs.Contains(e.PreviousValue)) {
						Inputs.Remove(e.PreviousValue);
					}
				}
				if (e.NewValue != null) {
					if (!Inputs.Contains(e.NewValue))
					{
						Inputs.Add(e.NewValue);
					}
				}
			}
		}

		/// <summary>
		/// Tries to get a map of whatever message body there is (POST/GET);
		/// </summary>
		/// <returns><c>true</c>, if get body map was tryed, <c>false</c> otherwise.</returns>
		/// <param name="httpInteraction">Http interaction.</param>
		/// <param name="givenData">Given data.</param>
		bool TryGetBodyMap (IHttpInteraction httpInteraction, out SerializingMap<object> givenData)
		{
			bool validRequest = false;
			string body = "";

			givenData = new SerializingMap<object> ();

			if (Method == httpInteraction.RequestMethod) {
				if (Method == "POST") {
					body = httpInteraction.RequestBody.ReadToEnd ();
					validRequest = body.Length > 0;
				} else if (Method == "GET") {
					string fullUrl = httpInteraction.URL [httpInteraction.URL.Count - 1];
					body = fullUrl.Substring (body.IndexOf ('?') + 1);
					validRequest = fullUrl.Length > body.Length;
				}
			}

			if (validRequest)
				givenData.AddFromString(body, HttpUtility.UrlDecode, '=', '&');

			return validRequest;
		}

		protected override bool Process (IInteraction parameters)
		{
			SerializingMap<object> data = new SerializingMap<object>();

			IHttpInteraction httpInteraction = parameters as IHttpInteraction;

			bool success = TryGetBodyMap (httpInteraction, out data);

			EntryInteraction entryInteraction = new EntryInteraction (Parent: parameters, Values: data);

			foreach(Service input in Inputs)
				success &= input.TryProcess(entryInteraction);
			
			// If all fields were cool with the new input, let them push 
			// the input (optionally parsed) into the main interaction, 
			// and branch off to the submission-branch.
			if (success) {
				entryInteraction.RaiseInputAccepted (parameters);
				success = Output.TryProcess(parameters);
			} else {
				FormattedWriter write = httpInteraction.ResponseBody.Write;

				FormTag.OpenWithDelegate(write);

				entryInteraction.RaiseFormDisplaying (
					Writer: httpInteraction.ResponseBody, 
					EntryAttempt: data.Length > 0);


				FormTag.CloseWithDelegate(write);

				success = true;
			}

			return success;
		}
	}
}
