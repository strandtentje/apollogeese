using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;
using BorrehSoft.Extensions.BasicWeblings.Site.Page;
using BorrehSoft.Utensils.Collections;

namespace BorrehSoft.Extensions.BasicWeblings
{
	/// <summary>
	/// Wrapper for Map of Template to enable simple template localization
	/// </summary>
	public class LocalizedTemplates : Service
	{
		private string title;
		private Template defaultTemplate;
		private Map<Template> localeTemplates = new Map<Template>();
		private string LocalizationKey { get; set; }
		private string FilenameLocalizationKey {
			get {
				return string.Format ("{{% {0} %}}", LocalizationKey);
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

		protected override void Initialize (Settings modSettings)
		{
			title = (string)modSettings ["title"];

			LocalizationKey = modSettings.GetString ("localizationkey", "locale");

			IEnumerable<object> localizations = (IEnumerable<object>)modSettings ["localizations"];

			foreach (object localization in localizations) {
				LoadLocalizedSubtemplate ((string)localization, modSettings, (string)modSettings["templatefile"]);
			}

			string defaultLocale;
			if (modSettings.TryGetString ("default", out defaultLocale)) {
				defaultTemplate = localeTemplates [defaultLocale];
			}
		}

		/// <summary>
		/// Loads a localized subtemplate.
		/// </summary>
		/// <param name="localeSettings">Locale settings.</param>
		private void LoadLocalizedSubtemplate(string localization, Settings globalSettings, string globalTemplatefile)
		{
			Template localeTemplate = new Template () { Branches = this.Branches };
			Settings localeSettings = globalSettings.Clone ();
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

