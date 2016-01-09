using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;
using BorrehSoft.Utensils.Collections;
using System.Collections.Generic;

namespace InputProcessing
{
	class MultipartKeyValueInteraction : SimpleInteraction, IRawInputInteraction
	{
		ReluctantTextReader TextReader;

		public MultipartKeyValueInteraction (
			IInteraction parameters, TextReader textReader, 
			string boundary, IEnumerable<string> fieldOrder) : base(parameters)
		{
			this.Feedback = new Map<Service> ();
			this.FieldOrder = fieldOrder;
			this.TextReader = new ReluctantTextReader (textReader);
		}

		public int InputCount { get; set; }

		public bool HasValuesAvailable { get; set; }

		public string CurrentName { get; set; }

		public IEnumerable<string> FieldOrder { private set; get; }

		public Map<Service> Feedback { get; set; }

		public bool HasReader() {
			return true;
		}

		public bool ReadNextName () {
			return false;
		}

		public object ReadInput() {
			return null;
		}

		public TextReader GetIncomingBodyReader() {
			return this.TextReader;
		}

		public void SkipInput() {

		}

		public void SetProcessedValue (object value) {

		}
	}
}