using System;
using BorrehSoft.ApolloGeese.CoreTypes;

using System.IO;
using BorrehSoft.Utensils.Parsing;
using System.Text;

namespace InputProcessing
{
	class UrlKeyValueInteraction : SimpleInteraction, IIncomingKeyValueInteraction
	{
		ReluctantTextReader dataReader;
		string currentName;

		public UrlKeyValueInteraction (IInteraction parent, TextReader dataReader) : base(parent)
		{
			this.dataReader = new ReluctantTextReader (dataReader);
		}

		public bool HasReader() {
			return true;
		}

		public TextReader GetIncomingBodyReader() {
			return this.dataReader;
		}

		public bool ReadNextName() {
			this.dataReader.StopCharacter = '=';

			if (-1 < this.dataReader.Peek ()) {
				this.currentName = this.dataReader.ReadToEnd ();

				this.dataReader.StopCharacter = '&';

				return true;
			} else {
				return false;
			}
		}

		public string GetCurrentName() {
			return this.currentName;
		}

		public void SetCurrentValue(object value) {
			this [GetCurrentName ()] = value;
		}

		public bool IsViewing { get; set; }
	}

}

