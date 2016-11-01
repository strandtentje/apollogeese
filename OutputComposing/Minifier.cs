using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using Yahoo.Yui.Compressor;
using BorrehSoft.Utensils.Collections.Maps;
using System.IO;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	public abstract class Minifier : Service
	{
		protected ICompressor compressor;
		Service Source;

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "source") {
				this.Source = e.NewValue;
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			var processor = StringProcessorInteraction.From (parameters);
			var success = this.Source.TryProcess (processor);
			processor.Run (this.compressor.Compress);
			return success;
		}
	}
}

