using System;
using System.Collections.Generic;
using System.Net;
using BorrehSoft.Utensils;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Log;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Collections.Maps;
using System.Text;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	public abstract class Configurable
	{
		public virtual IEnumerable<ConfigHint> GetConfigHints() {
			return new ConfigHint[] { };
		}

		/// <summary>
		/// The configuration of this service
		/// </summary>
		private Settings configuration;

		/// <summary>
		/// Will not fail elegantly when exception is thrown.
		/// </summary>
		public bool FailHard = false;

		public abstract string Description { get; }

		/// <summary>
		/// Gets the error message that caused 'SetSettings' to fail.
		/// </summary>
		/// <value>The error message.</value>
		public string InitErrorMessage { get; private set; }

		/// <summary>
		/// Gets more info on the error that caused SetSettings to fail.
		/// </summary>
		/// <value>The init error detail.</value>
		public string InitErrorDetail {
			get;
			private set;
		}

		/// <summary>
		/// Gets the settings.
		/// </summary>
		/// <returns>
		/// The settings.
		/// </returns>
		public Settings GetSettings ()
		{
			return configuration;
		}		

		public Settings Settings {
			get {
				return configuration;
			}
		}

		public virtual void LoadDefaultParameters(object defaultParameter) {
			LoadDefaultParameters ((string)defaultParameter);
		}

		public virtual void LoadDefaultParameters(string defaultParameter) {

		}

		protected virtual void Initialize(Settings settings) {

		}

		/// <summary>
		/// Tries to Initialize and leaves the an InitErrorMessage set if applicable.
		/// When no error is produced, the errormessage will remain blank.
		/// </summary>
		/// <returns><c>true</c>, if initialize was succesful, <c>false</c> otherwise.</returns>
		/// <param name="modSettings">Mod settings.</param>
		public bool SetSettings(Settings modSettings)
		{
			bool succesful;

			configuration = modSettings = modSettings ?? new Settings();

			try
			{
				if (modSettings.Has("default")) {
					LoadDefaultParameters(modSettings["default"]);
				}
				Initialize(modSettings);
				configuration.IsLoaded = true;
				InitErrorMessage = "";
				succesful = true;
			}
			catch(Exception ex) {
				InitErrorMessage = ex.Message;
				Secretary.Report (0, 
					string.Format (
						"Initialization for {2} {0} failed with the following message:\n{1}", 
						Description, InitErrorMessage, this.GetType().Name));

				StringBuilder errorDetail = new StringBuilder ();

				for (Exception inner = ex; inner != null; inner = inner.InnerException)
					errorDetail.AppendLine (inner.Message);

				InitErrorDetail = errorDetail.ToString ();

				Secretary.Report (0, errorDetail.ToString ());

				succesful = false;
			}

			return succesful;
		}
	}
}
