using System;
using System.Collections.Generic;
using System.Reflection;

namespace ModularFunk
{
	/// <summary>
	/// Tools for loading external modifications and additions.
	/// </summary>
	public static class ExternalMod
	{
		/// <summary>
		/// Gets the initiated types.
		/// </summary>
		/// <returns>
		/// The initiated types.
		/// </returns>
		/// <param name='assemblyFile'>
		/// Assembly file to get the initiated types from
		/// </param>
		/// <typeparam name='T'>
		/// The abstract to look for.
		/// </typeparam>
		public static List<T> GetInitiatedTypes<T> (string assemblyFile)
		{
			Assembly assembly = Assembly.LoadFile (assemblyFile);

			return GetInitiatedTypes<T>(assembly);
		}

		/// <summary>
		/// Gets the initiated types.
		/// </summary>
		/// <returns>
		/// The initiated types.
		/// </returns>
		/// <param name='assembly'>
		/// Assembly to get the initiated types from
		/// </param>
		/// <typeparam name='T'>
		/// The abstract to look for
		/// </typeparam>
		public static List<T> GetInitiatedTypes<T> (Assembly assembly)
		{
			List<T> initiatedTypes = new List<T>();
			foreach (Type potential in assembly.GetTypes()) 			
				if (typeof(T).IsAssignableFrom(potential))				
					initiatedTypes.Add((T)Activator.CreateInstance(potential));

			return initiatedTypes;
		}
	}
}

