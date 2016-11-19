using System;
using BorrehSoft.Utilities.Collections.Settings;
using System.Collections.Generic;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Log;
using System.IO;
using Parsing;
using System.Text;
using System.Text.RegularExpressions;
using BorrehSoft.Utilities.Collections.Maps;
using HttpMultipartParser;
using BorrehSoft.Utilities.Collections;

namespace ExternalData
{
	public class MultipartForm : TwoBranchedService
	{
		public override string Description {
			get {
				return "Multipart Form parser";
			}
		}

		const string MimeTypePrefix = "multipart/form-data; boundary=";

		private Service Mapped = Stub;

		[Instruction("Buffer size")]
		public int BufferSize { get; private set; }
		[Instruction("Whitelisted field names")]
		protected List<string> StringFieldWhiteList { get; private set; }

		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);

			this.BufferSize = settings.GetInt ("buffersize", 1024 * 64);
			this.StringFieldWhiteList = settings.GetStringList ("fieldlist");
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "mapped")
				this.Mapped = e.NewValue ?? Stub;
		}

		protected override bool Process (IInteraction parameters)
		{
			bool successful = true;

			IInteraction httpInteractionCandidate;
			if (successful = parameters.TryGetClosest (typeof(IHttpInteraction), out httpInteractionCandidate)) {
				IHttpInteraction httpInteraction = (IHttpInteraction)httpInteractionCandidate;

				if (httpInteraction.ContentType.StartsWith (MimeTypePrefix)) {
					string headerBoundary = httpInteraction.ContentType.Substring (MimeTypePrefix.Length);

					SimpleInteraction mappedValues = new SimpleInteraction (parameters);

					StreamingMultipartFormDataParser parser = new StreamingMultipartFormDataParser (
						httpInteraction.IncomingBody,
						headerBoundary, 
						httpInteraction.Encoding, 
						this.BufferSize
					);

					parser.ParameterHandler += delegate(ParameterPart part) {
						if (this.StringFieldWhiteList.Contains(part.Name)) {
							mappedValues[part.Name] = part.Data;

							successful &= this.Branches.Get(part.Name, Stub).TryProcess(
								new WwwInputInteraction(part.Name, part.Data, parameters)
							);
						}
					};

					parser.FileHandler += delegate(
						string name, 
						string fileName, 
						string contentType, 
						string contentDisposition, 
						byte[] buffer, 
						int bytes
					) {
						if (this.StringFieldWhiteList.Contains (name)) {
							MemoryStream stream = new MemoryStream (buffer, 0, bytes);

							SimpleIncomingInteraction incoming = new SimpleIncomingInteraction (
                                 stream,
                                 mappedValues, 
                                 contentDisposition,
                                 contentType
							);

							incoming ["mimetype"] = contentType;

							incoming ["filename"] = fileName;

							successful &= this.Branches.Get (name, Stub).TryProcess (incoming);

							stream.Dispose ();
						}
					};

					parser.Run ();

					successful &= this.Mapped.TryProcess (mappedValues);
				} else {
					Secretary.Report(5, "Require boundary content type for multipart forms");
					successful &= Failure.TryProcess (parameters);
				}
			} else {
				Secretary.Report(5, "Require HttpInteraction for multipart forms");
				successful &= Failure.TryProcess (parameters);
			}

			return successful;
		}
	}
}

