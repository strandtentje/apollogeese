using System;
using System.Collections.Generic;
using System.Reflection;
using BorrehSoft.Utensils.Log;

namespace BorrehSoft.Utensils.Collections.Maps
{
	/// <summary>
	/// Plugin collection.
	/// </summary>
	public class PluginCollection<T> : Map<Type>
	{
		/// <summary>
		/// Gets the constructed type by name.
		/// </summary>
		/// <returns>The constructed type.</returns>
		/// <param name="name">Name.</param>
		public T GetConstructed(string name)
		{
			return (T)Activator.CreateInstance (this[name]);
		}

		/// <summary>
		/// Produces a PluginCollection from one or multiple files
		/// </summary>
		/// <returns>PluginCollection</returns>
		/// <param name="files">Files.</param>
		public static PluginCollection<T> FromFiles(params string[] files)
		{
			PluginCollection<T> newcol = new PluginCollection<T> ();

			foreach (string file in files)
				newcol.AddFile (file);

			return newcol;
		}

		/// <summary>
		/// Adds all matching plugins from a file.
		/// </summary>
		/// <param name="file">File.</param>
		public void AddFile (string file)
		{			
			Secretary.Report (6, "Adding plugin from file:", file);
			AppendMatchingTypes (typeof(T), Assembly.LoadFrom (file));
		}

		/// <summary>
		/// Appends the matching types to a dictionary.
		/// </summary>
		/// <param name="registry">Dictionary to register newly matched types in.</param>
		/// <param name="match">Type to match with.</param>
		/// <param name="assembly">Assembly to load from.</param>
		public void AppendMatchingTypes (Type match, Assembly assembly)
		{
			foreach (Type potential in assembly.GetTypes())
				if (match.IsAssignableFrom (potential)) {
					Secretary.Report (7, "Type matched:", potential.Name);
					this[potential.Name] = potential;
				}
		}
	}
}
