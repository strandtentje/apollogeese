using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;
using System.Collections.Generic;
using BorrehSoft.Utilities.Collections;
using BorrehSoft.Utilities.Log;
using System.IO;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	/// <summary>
	/// Wrapper for Map of Template to enable simple template localization
	/// </summary>
	public class Translation : Service
	{
		private string title;
		private Template defaultTemplate;
		private Map<Template> localeTemplates = new Map<Template>();
		private string LocalizationKey { get; set; }
		private string FilenameLocalizationKey {
			get {
				return "%s";
			}
		}

		public override string Description {
			get {
				return title;
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{

		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			this.Settings ["file"] = defaultParameter;
		}

		protected override void Initialize (Settings modSettings)
		{
			LocalizationKey = modSettings.GetString ("localizationkey", "locale");

			IEnumerable<object> localizations = (IEnumerable<object>)modSettings ["locales"];

			string filename = (string)modSettings["file"];

			if (modSettings.Has ("title")) {
				title = (string)modSettings ["title"];
			} else { 
				FileInfo fileInfo = new FileInfo (filename);
				title = fileInfo.Name;
			}

			foreach (object localization in localizations) {
				LoadLocalizedSubtemplate ((string)localization, modSettings, filename);
			}
		}

		/// <summary>
		/// Loads a localized subtemplate.
		/// </summary>
		/// <param name="localeSettings">Locale settings.</param>
		private void LoadLocalizedSubtemplate(string localization, Settings globalSettings, string globalTemplatefile)
		{
			Template localeTemplate = new Template ();
			localeTemplate.SetBranches (this.Branches);
			Settings localeSettings = globalSettings.Clone ();

			if (localeSettings.Dictionary.ContainsKey ("default"))
				localeSettings.Dictionary.Remove ("default");

			localeSettings ["templatefile"] = globalTemplatefile.Replace (FilenameLocalizationKey, localization);
			localeTemplate.SetSettings (localeSettings);
			localeTemplates [localization] = localeTemplate;
			defaultTemplate = localeTemplate;
		}

		protected override bool Process (IInteraction parameters)
		{
			string templateKey;
			Template template = defaultTemplate;
			object localizedTemplate;

			if (parameters.TryGetFallbackString (LocalizationKey, out templateKey)) {
				if (localeTemplates.TryGetValue (templateKey, out localizedTemplate)) {
					template = (Template)localizedTemplate;
				} else {
					Secretary.Report (5, "No template for key", templateKey, "(", this.Description, ")");
				}
			} else {
				Secretary.Report (5, "No locale known at", this.Description);
			}

			return template.TryProcess (parameters);
		}
	}
}

