using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Log;
using System.IO;

namespace BorrehSoft.ApolloGeese.Loader
{
	/// <summary>
	/// Links and compiles into executable. Sort of. Mostly a rad name.
	/// I mean, it's got some plink plink going on.
	/// </summary>
	public class Complinker
	{
		/// <summary>
		/// Collection of plugin libraries; each lib may and probably does
		/// contain mulitple plugin services.
		/// </summary>
		private static PluginCollection<Service> plugins = new PluginCollection<Service> ();

		/// <summary>
		/// The branch name matcher. Not so neat. Maybe parameterise this later?
		/// </summary>
		private Regex branchNameMatcher = new Regex ("(.+)_branch");

		/// <summary>
		/// Gets the configuration.
		/// </summary>
		/// <value>The configuration.</value>
		public Settings Configuration { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.ApolloGeese.Duckling.Complinker"/> class using
		/// a configuration file defining service structure.
		/// </summary>
		/// <param name="config">Configuration file</param>
		public Complinker(string config)
		{
			Configuration = SettingsParser.FromFile (config);	
		}

		public void LoadPlugins ()
		{
			IEnumerable<object> pluginPathObjects = (Configuration ["plugins"] as IEnumerable<object>);

			foreach (object pluginPathObject in pluginPathObjects) {
				string pluginPath = (string)pluginPathObject;

				if (Directory.Exists (pluginPath)) {
					ScanDirectoryForPlugins (pluginPath);
				} else if (File.Exists (pluginPath)) {
					AddPluginFile (pluginPath);
				} else {
					Secretary.Report (4, "Path", pluginPath, "was not a file or a folder");
				}
			}
		}

		/// <summary>
		/// Gets the instances defined in the file
		/// </summary>
		/// <returns>The instances.</returns>
		public Map<Service> GetInstances()
		{
			Settings configurations = Configuration.GetSubsettings("instances");
			Map<Service> instances = new Map<Service> ();

			foreach (KeyValuePair<string, object> kvp in configurations.Dictionary) {
				instances[kvp.Key] = GetServiceForSettings ((Settings)kvp.Value);
            }

            Secretary.Report (5, "Loaded Instances from ", Configuration.SourceFile.Name);

			return instances;
		}

		/// <summary>
		/// Adds a plugin file.
		/// </summary>
		/// <param name="str">Filename.</param>
		public static void AddPluginFile (string str)
		{
			plugins.AddFile (str);
		}

		/// <summary>
		/// Scans the directory for plugins.
		/// </summary>
		/// <param name="pluginDirectoryPath">Plugin directory path.</param>
		void ScanDirectoryForPlugins (string pluginDirectoryPath)
		{
			IEnumerable<string> candidateDlls;
			candidateDlls = Directory.GetFiles (
				pluginDirectoryPath, "*.dll", SearchOption.TopDirectoryOnly);

			foreach (string candidateDll in candidateDlls) {
				try {
					AddPluginFile(candidateDll);
				} catch(Exception ex) {
					Secretary.Report (4, 
						"Failed to access file in directory:", candidateDll, 
						"due to", ex.Message, ". Continuing with next file.");
				}
			}
		}

		/// <summary>
		/// Connects a branch service to a service
		/// </summary>
		/// <param name="service">Service.</param>
		/// <param name="branchname">Branchname.</param>
		/// <param name="branchdata">Branchdata.</param>
		private void ConnectBranch (Service service, string branchname, Settings branchdata) {
			service.Branches [branchname] = GetServiceForSettings (branchdata);
		}

		/// <summary>
		/// Loads a tree of services in the existing context
		/// </summary>
		/// <returns>The tree.</returns>
		/// <param name="config">Config.</param>
		private Service GetServiceForSettings (Settings config)
		{
			string type;
			Settings moduleConfiguration;
			Service newService;
			bool succesfulInit, log;
			string[] logparams;

			if (config.Tag is Service) {
				newService = config.Tag as Service;
			} else {
				type = config.GetString ("type", "StubService");
				moduleConfiguration = (Settings)config ["modconf"];

				log = config.GetBool("log", false);
				logparams = config.GetString("logparams", "").Split(',');

				newService = plugins.GetConstructed (type);

				succesfulInit = newService.SetSettings (moduleConfiguration);

				newService.PossibleSiblingTypes = plugins;
				newService.FailHard = config.GetBool("fail", false);

				foreach (KeyValuePair<string, object> nameAndBranch in config.Dictionary) {
					Match branchName = branchNameMatcher.Match (nameAndBranch.Key);

					if (branchName.Success) ConnectBranch(
						newService, 
						branchName.Groups [1].Value, 
						nameAndBranch.Value as Settings);
				}

				if (config.Has ("branches")) {
					Settings branches = config.GetSubsettings ("branches");

					foreach (KeyValuePair<string, object> nameAndBranch in branches.Dictionary) 
						ConnectBranch (newService, nameAndBranch.Key, nameAndBranch.Value as Settings);
				}

				if (!succesfulInit) 
					Secretary.Report (5, type, " produced an error on initialization: ", newService.InitErrorMessage);


				config.Tag = newService;
			}


			return newService;
		}
	}
}

