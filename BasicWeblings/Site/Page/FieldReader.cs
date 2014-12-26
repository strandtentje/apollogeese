using System;
using System.Text;
using BorrehSoft.ApolloGeese.Duckling;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Text.RegularExpressions;
using BorrehSoft.ApolloGeese.Duckling.Http;
using System.Web;
using BorrehSoft.Utensils.Collections;

namespace BorrehSoft.Extensions.BasicWeblings
{
	public abstract class FieldReader : Service
	{
		public override string Description {
			get {
				if (FieldExpressions == null)
					return "None";
				else 
					return string.Format(string.Join(",", FieldExpressions.Keys));
			}
		}		

		public Dictionary<string, Regex> FieldExpressions { get; private set; }
		public Map<string> FieldDefaults { get; private set; }

		private Service Succesful, Form;
		private string failureVariable;
		bool htmlEscape, showFormBefore, showFormAfter, mapErrorStrings;

		protected override void Initialize (Settings modSettings)
		{
			Branches["successful"] = Stub;
			Branches["form"] = Stub;

			htmlEscape = modSettings.GetBool("escapehtml", true);
			showFormBefore = modSettings.GetBool("showformbefore", false);
			showFormAfter = modSettings.GetBool("showformafter", false);
			failureVariable = modSettings.GetString ("failurevariable", "");

			FieldExpressions = new Dictionary<string, Regex> ();
			FieldDefaults = new Map<string> ();

			RegexOptions regexOptions = RegexOptions.IgnoreCase;

			if (modSettings.GetBool ("casesensitive", false))
				regexOptions = RegexOptions.None;

			foreach (string fieldName in modSettings.Dictionary.Keys) {
				if (fieldName.StartsWith("field_")) {
					FieldExpressions.Add (fieldName.Substring(6), new Regex (modSettings [fieldName] as string, regexOptions));
				}

				if (fieldName.StartsWith ("default_")) {
					FieldDefaults [fieldName.Substring (8)] = modSettings [fieldName] as string;
				}
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "successful") Succesful = e.NewValue;
			if (e.Name == "form") Form = e.NewValue;
		}

		/// <summary>
		/// Deserialize the specified data.
		/// </summary>
		/// <param name="data">Data.</param>
		public abstract Map<object> Deserialize(string data);

		/// <summary>
		/// Acquires the data from nearest incoming body
		/// </summary>
		/// <returns>The data.</returns>
		/// <param name="parameters">Parameters.</param>
		public virtual string AcquireData (IInteraction parameters)
		{
			IIncomingBodiedInteraction request;
			request = (IIncomingBodiedInteraction)parameters.GetClosest(typeof(IIncomingBodiedInteraction));

			return request.IncomingBody.ReadToEnd ();
		}

		/// <summary>
		/// Processes the failures.
		/// </summary>
		/// <returns><c>true</c>, if failures were processed successfully, <c>false</c> otherwise.</returns>
		/// <param name="faultyFields">Fields with failures.</param>
		/// <param name="parameters">Parameters to use for calling failure branches.</param>
		private bool ProcessFailures(List<string> faultyFields, IInteraction parameters)
		{
			bool success = true;

			foreach (string fieldName in faultyFields) {
				string failName = string.Format ("{0}_failure", fieldName);
				if (Branches.Has (failName)) {
					Service failBranch = Branches [failName];
					success &= failBranch.TryProcess (parameters);
				}
			}

			return success;
		}

		/// <summary>
		/// Shows form for successful input
		/// </summary>
		/// <returns><c>true</c>, if successfully shown form, <c>false</c> otherwise.</returns>
		/// <param name="parsedData">Parsed data.</param>
		/// <param name="parameters">Parameters.</param>
		private bool DoSuccessfulForm(VerificationInteraction parsedData, IInteraction parameters)
		{
			bool success = true;
			
			if (showFormBefore) success &= Form.TryProcess(parsedData);
			success &= Succesful.TryProcess (parsedData);
			if (showFormAfter) success &= Form.TryProcess(parsedData);

			return success;
		}

		/// <summary>
		/// Shows form for faulty input
		/// </summary>
		/// <returns><c>true</c>, if successfully shown form, <c>false</c> otherwise.</returns>
		/// <param name="parsedData">Parsed data.</param>
		/// <param name="parameters">Parameters.</param>
		private bool DoFaultyForm(VerificationInteraction parsedData, IInteraction parameters)
		{
			bool success = true;

			if (failureVariable.Length > 0) {
				FailureWrapperInteraction failWrapParameters = new FailureWrapperInteraction (parameters);

				success &= ProcessFailures (parsedData.FaultyFields, failWrapParameters);

				parsedData [failureVariable] = failWrapParameters.GetTextAndClose ();
			} else {
				success &= ProcessFailures (parsedData.FaultyFields, parameters);
			}

			return success & Form.TryProcess(parsedData);
		}

		protected override bool Process (IInteraction parameters)
		{
			bool success = true; bool failures;
			Map<object> postData;
			VerificationInteraction parsedData;

			postData = Deserialize (AcquireData (parameters));
			parsedData = new VerificationInteraction (parameters, FieldExpressions) { HtmlEscape = htmlEscape };
			parsedData.LoadFields (postData, FieldDefaults);

			if (parsedData.FaultyFields.Count == 0) 
				success &= DoSuccessfulForm (parsedData, parameters);
			else 
				success &= DoFaultyForm (parsedData, parameters);

			return success;
		}

	}
}

