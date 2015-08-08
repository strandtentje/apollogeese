using System;
using System.IO;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace Testing
{
	class OutgoingTestableData : SimpleOutgoingInteraction, IDisposable
	{
		public OutgoingTestableData(IInteraction parent) : base(new MemoryStream(), parent)
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

