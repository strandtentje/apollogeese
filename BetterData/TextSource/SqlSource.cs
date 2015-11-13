using System;
using System.Collections.Generic;
using System.IO;
using BorrehSoft.Utensils.Parsing;
using System.Text;

namespace BetterData
{
	public abstract class SqlSource
	{
		public virtual bool IsOutdated {
			get {
				return false;
			}
		}

		public abstract string GetText();

		public bool UnchangedFlag { get; protected set; }

		private List<string> parameterNames = new List<string> ();

		string ReadOneParameterName (StringReader queryReader)
		{
			StringBuilder paramNameBuilder = new StringBuilder ();

			while (Parser.IsAlphaNumericUsc ((char)queryReader.Peek ())) {
				paramNameBuilder.Append ((char)queryReader.Read ());
			}

			return paramNameBuilder.ToString ();
		}

		public List<string> GetParameterNames() {
			if (!UnchangedFlag) {
				UnchangedFlag = true;

				this.parameterNames.Clear ();

				StringReader queryReader = new StringReader (GetText ());

				while (queryReader.Peek () > -1) {
					if (queryReader.Read () == '@') {
						string parName = ReadOneParameterName (queryReader);
						if (!this.parameterNames.Contains (parName)) {
							this.parameterNames.Add (parName);
						}
					}
				}
			}

			return this.parameterNames;
		}
	}
}

