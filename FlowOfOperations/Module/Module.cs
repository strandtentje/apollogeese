using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.ApolloGeese.Loader;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Log;
using System.Collections.Generic;
using System.IO;

namespace BorrehSoft.ApolloGeese.Extensions.FlowOfOperations.Module
{
	/// <summary>
	/// Executes into branch from another file.
	/// </summary>
	public class Module : Service
	{
		const string OverrideSuffix = "_override";

		private static class SettingsKeys
		{
			public const string 
				File = "file",
				Branch = "branch",
				BranchVariable = "branchvariable",
				BranchName = "branchname",
				AutoInvoke = "autoinvoke",
				InjectOwnSettings = "injectownsettings",
				Remap = "remap",
				Reassignments = "reassignments",
				WorkingDirectory = "rootpath";

			public static bool Contains (string candidateSetting)
			{
				switch (candidateSetting) {
				case File:
				case Branch:
				case BranchVariable:
				case BranchName:
				case AutoInvoke:
				case InjectOwnSettings:
				case Remap:
				case Reassignments:
				case WorkingDirectory:
					return true;
				default:
					return false;
				}
			}
		}

		public override string Description {
			get {
				if (BranchName == null)
					return string.Format ("{0}:directed", File);
				else
					return string.Format ("{0}:{1}", File, BranchName);
			}
		}

		[Instruction ("Module file to load.")]
		public string File { get; set; }

		[Instruction ("Alternative working directory for using when parsing module", null)]
		public string WorkingDirectory { get; set; }

		[Instruction ("Name of branch in module to fire up.")]
		public string BranchName { get; set; }

		[Instruction ("Name of the context variable wherefrom the branch name should be acquired")]
		public string BranchVariable { get; set; }

		[Instruction ("Flag indicating if this module may propagate its own settings into context", false)]
		public bool InjectOwnSettings { get; set; }

		/// <summary>
		/// The variable overrides.
		/// </summary>
		private Map<string> VariableOverrides = new Map<string> ();

		/// <summary>
		/// The variable injections.
		/// </summary>
		private Map<object> VariableInjections = new Map<object> ();

		public override void LoadDefaultParameters (string defaultParameter)
		{
			string[] pathAndBranch = defaultParameter.Split ('@');

			this.Settings [SettingsKeys.File] = pathAndBranch [0];

			if (pathAndBranch.Length > 1) {
				this.Settings [SettingsKeys.Branch] = pathAndBranch [1]; 
			}
		}

		protected override void Initialize (Settings modSettings)
		{
			this.File = modSettings.GetString (SettingsKeys.File);
			this.BranchName = modSettings.GetString (SettingsKeys.Branch, null);			
			this.BranchVariable = modSettings.GetString (SettingsKeys.BranchVariable, SettingsKeys.BranchName);
			this.AutoInvoke = modSettings.GetBool (SettingsKeys.AutoInvoke, false);
			this.InjectOwnSettings = modSettings.GetBool (SettingsKeys.InjectOwnSettings, false);
			this.WorkingDirectory = modSettings.GetString (
				SettingsKeys.WorkingDirectory,
				(new FileInfo (this.File)).DirectoryName);

			RegisterVariableOverrides (modSettings.GetSubsettings (SettingsKeys.Reassignments));
			RegisterVariableOverrides (modSettings.GetSubsettings (SettingsKeys.Remap));

			foreach (KeyValuePair<string, object> setting in modSettings.Dictionary) {
				if (IsOverride (setting)) {					
					int keyTailLength = setting.Key.Length - OverrideSuffix.Length;
					string targetVariable = setting.Key.Substring (0, keyTailLength);
					this.VariableOverrides [targetVariable] = (string)setting.Value;
				} 
				if (IsInjectable (setting)) {
					VariableInjections [setting.Key] = setting.Value;
				}
			}

		}

		/// <summary>
		/// Determines whether this instance is override the specified setting.
		/// </summary>
		/// <returns><c>true</c> if this instance is override the specified setting; otherwise, <c>false</c>.</returns>
		/// <param name="setting">Setting.</param>
		private bool IsOverride (KeyValuePair<string, object> setting)
		{
			return setting.Key.EndsWith (OverrideSuffix) && setting.Value is string;
		}

		/// <summary>
		/// Determines whether this instance is injectable the specified setting.
		/// </summary>
		/// <returns><c>true</c> if this instance is injectable the specified setting; otherwise, <c>false</c>.</returns>
		/// <param name="setting">Setting.</param>
		private bool IsInjectable (KeyValuePair<string, object> setting)
		{
			bool isUnsafe = SettingsKeys.Contains (setting.Key) || setting.Key.EndsWith (OverrideSuffix);
			return !isUnsafe || this.InjectOwnSettings;
		}

		/// <summary>
		/// Registers the variable overrides.
		/// </summary>
		/// <param name="overrides">Overrides.</param>
		private void RegisterVariableOverrides (Map<object> overrides)
		{
			foreach (KeyValuePair<string, object> overrideCandidate in overrides.Dictionary) {
				if (overrideCandidate.Value is string) {
					this.VariableOverrides [overrideCandidate.Key] = (string)overrideCandidate.Value;
				}
			}
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{

		}

		public override void OnReady ()
		{
			if (this.AutoInvoke) {
				if (this.BranchName != null) {
					if (!GetService (this.BranchName).TryProcess (CreateJump ())) {
						Secretary.Report (5, "Autoinvoke branch", this.BranchName, "failed");
					}
				} else {
					Secretary.Report (5, "Can't autoinvoke", Description, "- branch needs explicit stating.");
				}
			}
		}

		public Service GetService (string branchName)
		{
			return ServiceCollectionCache.Get (File, WorkingDirectory).Get (branchName, null);
		}

		public bool AutoInvoke { get; private set; }

		class ModuleBranchException : Exception
		{
			public ModuleBranchException (string branchVariable) : base (
					string.Format ("No branch name found in context at {0}", 
						branchVariable))
			{

			}
		}

		protected override bool Process (IInteraction parameters)
		{
			Service referredService = null;
			JumpInteraction jumpInteraction = null;

			string pickedBranchName = "";

			if (BranchName == null) {
				if (!parameters.TryGetFallbackString (this.BranchVariable, out pickedBranchName)) {
					throw new ModuleBranchException (this.BranchVariable);
				}
			} else {
				pickedBranchName = BranchName;
			}

			referredService = GetService (pickedBranchName);

			if (referredService == null) {
				throw new NullReferenceException (
					string.Format ("Branch with name '{0}' was not found among [{1}]",
						pickedBranchName, 
						File
					)
				);
			} else {
				jumpInteraction = CreateJump (parameters);
			}

			return referredService.TryProcess (jumpInteraction);
		}

		private JumpInteraction CreateJump (IInteraction parameters = null)
		{
			return new JumpInteraction (parameters,
				this.Branches, this.VariableOverrides, this.VariableInjections);
		}
	}
}
	