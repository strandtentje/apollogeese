using System;
using BorrehSoft.ApolloGeese.CoreTypes;

using System.IO;
using BorrehSoft.Utensils.Parsing;
using System.Text;
using BorrehSoft.Utensils.Collections;

namespace InputProcessing
{
	class UrlKeyValueInteraction : SimpleInteraction, IRawInputInteraction
	{
		ReluctantTextReader dataReader;

		public Map<Service> Feedback { get; set; }

		string currentName;

		public UrlKeyValueInteraction (IInteraction parent, TextReader dataReader) : base(parent)
		{
			this.Feedback = new Map<Service> ();
			this.dataReader = new ReluctantTextReader (dataReader);
		}
		
		public int InputCount { get; set; }

		public bool HasValuesAvailable { get; set; }
		
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

		public object ReadInput() {
			return this.dataReader.ReadToEnd();
		}

		public string GetCurrentName() {
			return this.currentName;
		}

		public void SetProcessedValue(object value) {
			this [GetCurrentName ()] = value;
		}
	}
}

