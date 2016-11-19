using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.Module;
using BorrehSoft.Utilities.Collections;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;

namespace BorrehSoft.ApolloGeese.Extensions.InputProcessing
{
	/// <summary>
	/// Generic FieldReader; intended for inheritance, like GetFieldReader and PostFieldReader do.
	/// </summary>
	public abstract class FieldReader : Service
	{
		public override string Description {
			get {
				if (FieldExpressions == null)
					return "None";
				else 
					return string.Join(",", FieldExpressions.Keys);
			}
		}		

		/// <summary>
		/// Gets the field verification regular expressions.
		/// </summary>
		/// <value>The field expressions.</value>
		public Dictionary<string, Regex> FieldExpressions { get; private set; }

		/// <summary>
		/// Gets the field default values for missing fields
		/// </summary>
		/// <value>The field defaults.</value>
		public Map<string> FieldDefaults { get; private set; }

		/// <summary>
		/// Get the names of the fields that are allowed to have their values 
		/// be filled in or preserved from existing context
		/// </summary>
		/// <value>The interaction fallback names.</value>
		public List<string> InteractionFallbackNames { get; private set; }

		private Service Succesful, Form;
		private Module FailureHandler;
		bool HtmlEscape;
		bool ShowFormBefore;
		bool ShowFormAfter;
		bool MapErrorStrings;

		protected override void Initialize (Settings modSettings)
		{
			Branches["successful"] = Stub;
			Branches["form"] = Stub;

			HtmlEscape = modSettings.GetBool("escapehtml", true);
			ShowFormBefore = modSettings.GetBool("showformbefore", false);
			ShowFormAfter = modSettings.GetBool("showformafter", false);
			MapErrorStrings = modSettings.GetBool ("maperrors", false);

			FieldExpressions = new Dictionary<string, Regex> ();
			FieldDefaults = new Map<string> ();
			InteractionFallbackNames = new List<string> ();

			if (modSettings.Has("allowfallback"))
				foreach (object fbName in (IEnumerable<object>)modSettings["allowfallback"])
					InteractionFallbackNames.Add ((string)fbName);

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
			if (e.Name == "failhandle")
				FailureHandler = (Module)e.NewValue;
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

			return request.GetIncomingBodyReader().ReadToEnd ();
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
			
			if (ShowFormBefore) success &= Form.TryProcess(parsedData);
			success &= Succesful.TryProcess (parsedData);
			if (ShowFormAfter) success &= Form.TryProcess(parsedData);

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

			Encoding encoding;

			IInteraction outgoingCandidate;
			if (parameters.TryGetClosest (typeof(IOutgoingBodiedInteraction), out outgoingCandidate)) {
				IOutgoingBodiedInteraction outgoing = (IOutgoingBodiedInteraction)outgoingCandidate;

				encoding = outgoing.Encoding;
			} else {
				encoding = Encoding.UTF8;
			}
			
			foreach (string fieldName in parsedData.FaultyFields) {
				string failName = string.Format ("{0}_failure", fieldName);

				Service handler; 
				FailureWrapperInteraction fwInteraction = null; 
				IInteraction interaction = parameters;

				handler = FailureHandler ?? Branches [failName];

				if (handler != null) {
					if (MapErrorStrings) {
						interaction = fwInteraction = new FailureWrapperInteraction (parameters, encoding);
					}

					if (FailureHandler != null) {
						SimpleInteraction failureInteraction;
						interaction = failureInteraction = new SimpleInteraction (interaction);
						failureInteraction ["failurename"] = failName;
					}

					success &= handler.TryProcess (interaction);

					if (fwInteraction != null) {
						parsedData [failName] = fwInteraction.GetTextAndClose ();
					}
				}
			}

			return success & Form.TryProcess(parsedData);
		}

		protected virtual string GetSourceName (IInteraction parameters) {
			IIncomingBodiedInteraction data;
			data = (IIncomingBodiedInteraction)parameters.GetClosest (typeof(IIncomingBodiedInteraction));

			return data.SourceName;
		}

		protected override bool Process (IInteraction parameters)
		{
			bool success = true;
			object postDataObject;
			VerificationInteraction parsedData;

			string sourceName = GetSourceName (parameters);

			parsedData = new VerificationInteraction (parameters, sourceName, FieldExpressions, InteractionFallbackNames) { HtmlEscape = HtmlEscape };

			if (parameters.TryGetFallback (sourceName, out postDataObject)) {
				parsedData [sourceName] = (Map<object>)postDataObject;
			} else {
				parsedData [sourceName] = Deserialize (AcquireData (parameters));
			}

			parsedData.LoadFields (FieldDefaults);

			if (parsedData.FaultyFields.Count == 0) 
				success &= DoSuccessfulForm (parsedData, parameters);
			else 
				success &= DoFaultyForm (parsedData, parameters);

			return success;
		}

	}
}

