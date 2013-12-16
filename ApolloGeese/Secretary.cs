using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils;
using BorrehSoft.Utensils.Settings;

namespace BorrehSoft.ApolloGeese
{
	/// <summary>
	/// Secretary; initiator of symbiosis and maintainer
	/// of logfiles.
	/// </summary>
	public class Secretary
	{
		static int globVerbosity = 10;

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		static void Main (string[] args)
		{
			HttpServer servicePort = new HttpServer("http://*:8080/");
			Settings configuration = Settings.FromFile ("apollogeese.conf");

			foreach (string dll in (List<string>)configuration["plugins"]) {
				ServiceProvider provider = 
					ExternalMod.GetInitiatedTypes<ServiceProvider> (dll);
				servicePort.AddService (provider);
			}

			Console.ReadLine();
		}

		/// <summary>
		/// Report messageParts with specified verbosity.
		/// </summary>
		/// <param name='verbosity'>
		/// Verbosity level.
		/// </param>
		/// <param name='messageParts'>
		/// Message parts.
		/// </param>
		public static void Report (int verbosity, params string[] messageParts)
		{
			if (verbosity < globVerbosity) {
				Console.Write (DateTime.Now.ToString ("HH:MM:SS | "));
				Console.WriteLine (string.Join (" ", messageParts));
			}
		}

	}
}
