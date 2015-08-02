using System;
using System.Text;
using System.Collections.Generic;
using BorrehSoft.Utensils;
using System.IO;
using System.Diagnostics;

namespace BorrehSoft.Utensils.Collections.Maps
{
	/// <summary>
	/// A regular map with added functionality of parsing and composing.
	/// </summary>
	public class SerializingMap<T> : Map<T>
	{
		public delegate T Parser(string data);

		/// <summary>
		/// Adds elements from string stream.
		/// </summary>
		/// <param name="data">Data to parse</param>
		/// <param name="parser">String parser for element value.</param>
		/// <param name="assigner">Value-assigning character.</param>
		/// <param name="concatenator">Concatenator of assignments.</param>
		public void AddFromString (string source, Parser parser, char assigner, char concatenator, long limit = -1)
		{
			StringBuilder buffer = new StringBuilder ();
			Stopwatch timeLimiter = null;

			if (limit > -1) {
				timeLimiter = new Stopwatch ();
				timeLimiter.Start();
			}

			string identifier = "";

			bool expectingAssigner = true;

			foreach(char c in source)
			{
				if (c == concatenator)
				{
					expectingAssigner = true;
					this [identifier] = parser (buffer.ToString ());
					buffer.Clear ();
				}
				else if (expectingAssigner && (c == assigner))
				{
					expectingAssigner = false;
					identifier = buffer.ToString ();
					buffer.Clear();
				}
				else {
					buffer.Append(c);
				}

				if (timeLimiter != null)
				{
					if (timeLimiter.ElapsedMilliseconds > limit)
					{
						timeLimiter.Stop();
						throw new TimeoutException();
					}
				}
			}

			if (buffer.Length > 0) {
				this [identifier] = parser (buffer.ToString ());
				buffer.Clear ();
			}
		}

		public void WriteUsingCallback (FormattedWriter writeMethod, string format)
		{
			foreach (KeyValuePair<string, T> kvp in base.Dictionary) 
				writeMethod(format, kvp.Key, kvp.Value.ToString());

		}

		public void WriteUsingCallback (FormattedWriter writeMethod, Map<T> overrides, string format)
		{
			T value;

			foreach (KeyValuePair<string, T> kvp in base.Dictionary) {
				if (overrides.Dictionary.ContainsKey (kvp.Key)) 
					value = overrides.Dictionary [kvp.Key];
				else
					value = kvp.Value;
				writeMethod (format, kvp.Key, value.ToString ());
			}

			foreach (KeyValuePair<string, T> kvp in overrides.Dictionary) {

			}
		}


		/// <summary>
		/// Writes the pairs using the supplied write method.
		/// </summary>
		/// <param name="writeMethod">Write method.</param>
		/// <param name="connector">Connector.</param>
		/// <param name="seperator">Seperator.</param>
		public void WriteUsingCallback(FormattedWriter writeMethod, char connector, char seperator)
		{
			WriteUsingCallback(writeMethod, "{0}" + connector + "{1}" + seperator);
		}

		/// <summary>
		/// Writes the pairs to the supplied stringbuilder using a format.
		/// </summary>
		/// <param name="builder">Builder.</param>
		/// <param name="format">Format.</param>
		public void WriteUsingCallback(StringBuilder builder, string format)
		{
			FormattedWriter sblWriter = delegate(string f, string[] parameters) {
				builder.AppendFormat(f, parameters);
			};

			WriteUsingCallback(sblWriter, format);
		}
	}
}

