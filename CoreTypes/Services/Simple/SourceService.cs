using System;
using BorrehSoft.Utensils.Collections.Settings;
using System.Text;
using BorrehSoft.Utensils.Collections.Maps;
using System.IO;


namespace BorrehSoft.ApolloGeese.CoreTypes
{
	public class SourceService : TwoBranchedService
	{
		private Service SinkingBranch = null;

		private Encoding FallbackEncoding;

		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);
			this.FallbackEncoding = Encoding.GetEncoding (
				settings.GetString (
					"fallbackencoding", 
					"utf-8"
				)
			);
		}

		protected delegate void WriteCallback(StreamWriter writer);

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "sink")
				this.SinkingBranch = e.NewValue;
		}

		private void WriteToParameters(IInteraction parameters, WriteCallback callback)
		{
			IInteraction sinkInteraction;

			if (parameters.TryGetClosest (
				    typeof(IOutgoingWriterInteraction),
				    sinkInteraction)) {
				IOutgoingWriterInteraction writerSink = (IOutgoingWriterInteraction)sinkInteraction;

				callback (writerSink.GetOutgoingBodyWriter ());
			} else if (parameters.TryGetClosest (
				           typeof(IOutgoingBodiedInteraction), 
				           sinkInteraction)) {
				IOutgoingBodiedInteraction bodiedSink = (IOutgoingBodiedInteraction)sinkInteraction;

				using (StreamWriter writer = new StreamWriter (
					                             bodiedSink.OutgoingBody, 
					                             bodiedSink.Encoding
				                             )) {
					callback (writer);
				}
			} else {
				throw new Exception ("No Outgoing Streams available");
			}
		}

		protected void StartWriting(IInteraction parameters, WriteCallback callback) 
		{
			if (SinkingBranch == null) {
				WriteToParameters (parameters, callback);
			} else {

			}
		}
	}
}

