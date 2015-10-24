using System;
using BorrehSoft.ApolloGeese.CoreTypes;

using System.IO;
using BorrehSoft.Utensils.Parsing;
using System.Text;
using BorrehSoft.Utensils.Collections;
using System.Collections.Generic;

namespace InputProcessing
{
	class UrlKeyValueInteraction : SimpleInteraction, IRawInputInteraction
	{
		ReluctantTextReader dataReader;

		public Map<Service> Feedback { get; set; }

		public UrlKeyValueInteraction (IInteraction parent, TextReader dataReader, IEnumerable<string> fieldOrder) : base(parent)
		{
			this.Feedback = new Map<Service> ();
			this.dataReader = new ReluctantTextReader (dataReader);
			this.FieldOrder = fieldOrder;
		}

		public IEnumerable<string> FieldOrder { get; private set; }

		public int InputCount { get; set; }

		public bool HasValuesAvailable { get; set; }

		public string CurrentName { get; set; }
		
		public bool HasReader() {
			return true;
		}

		public TextReader GetIncomingBodyReader() {
			return this.dataReader;
		}

		public bool ReadNextName() {
			this.dataReader.StopCharacter = '=';

			if (-1 < this.dataReader.Peek ()) {

				if ((char)this.dataReader.Peek () == '&')
					this.dataReader.Read ();

				this.CurrentName = this.dataReader.ReadToEnd ();

				this.dataReader.StopCharacter = '&';

				return true;
			} else {
				return false;
			}
		}

		public object ReadInput() {
			if ((char)this.dataReader.Peek () == '=')
				this.dataReader.Read ();

			return this.dataReader.ReadToEnd();
		}

		public void SkipInput() {
			this.dataReader.SkipToEnd ();
		}

		public void SetProcessedValue(object value) {
			this [this.CurrentName] = value;
		}
	}
}

