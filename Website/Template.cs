using System;
using System.Text.RegularExpressions;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Settings;
using System.Net;
using System.IO;
using BorrehSoft.Utensils;
using Stringtionary = System.Collections.Generic.Dictionary<string, string>;

namespace Website
{
	/// <summary>
	/// Simple template service which fills 
	/// </summary>
	public class Template : Service
	{
		private Settings settings;
		/// <summary>
		/// Regex Matches that may be replaced
		/// </summary>
		private MatchCollection replaceables;
		/// <summary>
		/// Variables this template supports
		/// </summary>
		private StringList templateVariables = new StringList();

		private string templateFile, rawTemplate, chunkPattern, title;

		/// <summary>
		/// HTML Template Service will expose Branches which have been defined
		/// in the template (if set)
		/// </summary>
		/// <value>The advertised branches.</value>
		public override string[] AdvertisedBranches {
			get {
				return new string[] {};
			}
		}

		/// <summary>
		/// Returns the title with this service.
		/// </summary>
		/// <value>The title of this page</value>
		public override string Description {
			get { return title; }
		}

		protected override void Initialize (Settings modSettings)
		{
			title = (string)modSettings ["title"];
			chunkPattern = (string)modSettings ["chunkpattern"];
			templateFile = (string)modSettings ["templatefile"];
			settings = modSettings;

			if (chunkPattern == null) chunkPattern = @"\{% ([a-z]+) %\}";
			rawTemplate = File.ReadAllText (templateFile);

			replaceables = templateVariables.AddUniqueRegexMatches (rawTemplate, chunkPattern);
		}

		protected override bool Process (Interaction parameters)
		{
			foreach (string variableName in templateVariables) {
				if (parameters.Luggage [variableName] == null) {
					// Seperate the parameters so the template variable contents
					// do not get appended to the body of the response.
					Interaction safeParameters = parameters.Clone ();
					// Default the variable template value to an empty string
					string variableValue = "";

					// Branch off and assign the result to the variable, otherwise
					// asign a default value if available.
					if (RunBranch (variableName, safeParameters)) {
						variableValue = safeParameters.HTML.ToString ();
						if (parameters.SetAlternativeBody (safeParameters))
						// SUPRISE EXIT FROM THE ROUTINE!
						// This happens if a branch is offering an
						// alternative datastream to read from.
							return true; 

					} else if (settings[variableName] != null) {
						variableValue = (string)settings [variableName];
					}

					parameters.Luggage [variableName] = variableValue;
				}
			}

			int cursor = 0;
			string groupName, lugValue;

			try	{
				foreach (Match replaceable in replaceables) {
					// Append from:
					//  a) The beginning of the document
					//  b) The continuation of the document since the last chunk
					// to:
					//	a) The beginning of a chunk
					//  b) The end of the document
					parameters.HTML.Append (
						rawTemplate.Substring (cursor, replaceable.Index - cursor));

					groupName = replaceable.Groups[1].Value;
					lugValue = (string)parameters.Luggage [groupName];

					parameters.HTML.Append (lugValue);

					cursor = replaceable.Index + replaceable.Length;
				}

				// In the very likely even the cursor is not at the end of the document,
				// the last it of the document need to be written to the body as well.
				if (cursor < rawTemplate.Length)
					parameters.HTML.Append(rawTemplate.Substring(cursor));

				return true;
			}
			catch (Exception ex) {
				// Forward the exception but append a cursor position and a message
				// so the user won't be entirely in the dark.
				throw new Exception (
					string.Format ("Replacing template variables failed at cursor position {0}, {2}:\n{1}",
				               cursor.ToString (), rawTemplate.Substring (cursor, 40), ex.Message), ex);
			}

			return false;
		}
	}
}

