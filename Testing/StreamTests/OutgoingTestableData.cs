using System;
using System.IO;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections.Maps;
using System.Text;

namespace Testing
{
	class OutgoingTestableData : SimpleOutgoingInteraction, IDisposable
	{
		public OutgoingTestableData(IInteraction parent) : base(new MemoryStream() , parent) {

		}

		public OutgoingTestableData(IInteraction parent, 
			Encoding encoding) : base(new MemoryStream(), encoding, parent)
		{

		}

		public void Dispose() {
			if (HasWriter ()) {
				this.GetOutgoingBodyWriter ().Dispose ();
			} else {
				this.OutgoingBody.Dispose ();
			}
		}

		public Stream GetProduct()
		{
			this.OutgoingBody.Position = 0;
			return this.OutgoingBody;
		}
	}
}

