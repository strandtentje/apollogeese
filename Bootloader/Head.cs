using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections;
using System.Collections.Generic;
using System.Reflection;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Log;
using BorrehSoft.Utensils.Collections.Settings;
using System.Text.RegularExpressions;
using BorrehSoft.Utensils;
using BorrehSoft.ApolloGeese.Loader;

namespace BorrehSoft.ApolloGeese
{
	/// <summary>
	/// Secretary. Fires up a webserver and loads in the modules according
	/// to configuration. Will keep logg (Log may be kept by calling Secretary.Report
	/// </summary>
	public class Head
	{
		static void StartLog(string folder)
		{
			string time = DateTime.Now.ToString ("yyyy-MM-dd--THHmmsszz");

			Secretary logger = new Secretary (string.Format ("{0}/{1}.log",	folder, time));

			logger.globVerbosity = 10;

			logger.ReportHere (0, "Logfile Opened");
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		static void Main (string[] args)
		{
			Queue<string> CommandLineArguments = new Queue<string> (args);

			string config = "apollogeese.conf", logfolder = ".", runbranch = "";
			bool pluginsFromConfig = true;
			bool pluginsFromBin = false;
			SimpleInteraction miscArgs = new SimpleInteraction ();

			StartLog(logfolder);

			while (CommandLineArguments.Count > 0) {
				string paramAhead = CommandLineArguments.Dequeue();
				if (paramAhead.ToLower () == "-b") {
					runbranch = CommandLineArguments.Dequeue ();
					Secretary.Report (5, "Branch name: ", runbranch);
				} else if (paramAhead.ToLower () == "-c") {
					config = CommandLineArguments.Dequeue ();
					Secretary.Report (5, "Config file: ", config);
				} else if (paramAhead.ToLower () == "-l") {
					logfolder = CommandLineArguments.Dequeue ();
					Secretary.Report (5, "Logfile: ", config);				
				} else if (paramAhead.ToLower () == "-pfc") {
					pluginsFromConfig = bool.Parse (CommandLineArguments.Dequeue ());
					Secretary.Report (5, "Plugins from Config: ", pluginsFromConfig.ToString());
				} else if (paramAhead.ToLower () == "-pfb") {
					pluginsFromBin = bool.Parse (CommandLineArguments.Dequeue ());
					Secretary.Report (5, "Plugins from Bin: ", pluginsFromBin.ToString());
				} else if (runbranch.Length > 0) {
					string[] pair = paramAhead.ToLower ().Split ('=');

					if (pair.Length == 2) {
						miscArgs [pair [0]] = pair [1];
						Secretary.Report (5, pair [0], "=", pair [1]);
					} else {
						throw new Exception (
							"Interaction variables need to be supplied as key=value-pair");
					}
				} else {
					throw new Exception ("Not sure what to do with " + paramAhead.ToLower ());
				}
			}
		
			Map<Service> services = InstanceLoader.GetInstances (config, pluginsFromConfig, pluginsFromBin);

			if (runbranch.Length > 0) {
				services [runbranch].TryProcess (miscArgs);
			}
		}
	}
}
