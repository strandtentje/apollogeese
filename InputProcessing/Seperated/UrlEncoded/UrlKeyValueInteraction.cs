using System;
using BorrehSoft.ApolloGeese.CoreTypes;

using System.IO;
using BorrehSoft.Utensils.Parsing;
using System.Text;

namespace InputProcessing
{
	class UrlKeyValueInteraction : SimpleInteraction, IIncomingKeyValueInteraction
	{
		TextReader dataReader;
		string currentName;

		public UrlKeyValueInteraction (IInteraction parent, TextReader dataReader) : base(parent)
		{
			this.dataReader = dataReader;
		}

		public bool HasReader() {
			return true;
		}

		public TextReader GetIncomingBodyReader() {
			return this.dataReader;
		}

		public bool ReadNextName() {
			int currentCharcode;
			char currentChar;
			StringBuilder newName = new StringBuilder ();

			while ((currentCharcode = this.dataReader.Read()) >= 0) {
				currentChar = (char)currentCharcode;

				if (currentChar == '=') {
					break;
				} else {
					newName.Append (currentChar);
				}
			}

			this.currentName = newName.ToString();

			return newName.Length > 0;
		}

		public string GetCurrentName() {
			return this.currentName;
		}

		public bool Finalized { get; set; }
	}

}

