using System;

namespace ApolloGeese
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
			HttpServer n = new HttpServer("http://*:8080/");

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
