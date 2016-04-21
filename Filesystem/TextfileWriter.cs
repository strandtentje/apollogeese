using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Text;

namespace Filesystem
{
	public class TextfileWriter : Service
	{
		public string DirectoryPath { get; private set; }

		public string Filename { get; private set; }

		public int BufferSize { get; private set; }

		public bool HasGeneratedFilename { get { return Filename.Length == 0; } }

		private Service Source = null;

		private Service Success = Stub;

		private Encoding Encoding;

		public override string Description {
			get {
				string description;

				if (this.HasGeneratedFilename) {
					description = string.Format ("Writes uniquely named file to directory {0}",
						this.DirectoryPath);
				} else {
					description = string.Format ("Writes file to directory {0}, using name {1}",
						this.DirectoryPath, this.Filename);
				}

				return description;
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			if (File.Exists (defaultParameter)) {
				FileInfo info = new FileInfo (defaultParameter);

				if (info.Exists) {
					this.DirectoryPath = info.DirectoryName;
					this.Filename = info.Name;
				}
			} else if (Directory.Exists (defaultParameter)) {
				this.DirectoryPath = defaultParameter;
				this.Filename = "";
			}
		}

		protected override void Initialize (Settings settings)
		{
			this.DirectoryPath = settings.GetString ("rootpath");
			this.Filename = settings.GetString ("filename", "");
			this.Encoding = Encoding.GetEncoding(settings.GetString("encoding", "utf-8"));
			this.BufferSize = settings.GetInt ("buffersize", 1024 * 128);
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "source")
				this.Source = e.NewValue;
			else if (e.Name == "success")
				this.Success = e.NewValue;
		}

		private string GenerateUniqueFilename() {
			return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("=","_").Replace("+", "-");
		}

		private string PrependDirectory(string fileName) {
			return string.Format ("{0}{1}{2}", this.DirectoryPath, Path.PathSeparator, fileName);
		}

		private string ComposeSinkFilename() {
			string composedFilename;

			if (this.Filename.Length == 0) {
				string fileName;
				do {
					fileName = GenerateUniqueFilename ();
					composedFilename = PrependDirectory (fileName);
				} while(File.Exists (composedFilename));
			} else {
				composedFilename = PrependDirectory (this.Filename);
			}

			return composedFilename;
		}

		protected override bool Process (IInteraction parameters)
		{
			TextReader sourceReader;
			bool success = true;

			if (Source == null) {
				IInteraction sourceInteraction;
				success &= parameters.TryGetClosest (typeof(IIncomingReaderInteraction), out sourceInteraction);
				if (success) sourceReader = ((IIncomingReaderInteraction)sourceInteraction).GetIncomingBodyReader ();				
			} else {
				SimpleOutgoingInteraction sourceInteraction = new SimpleOutgoingInteraction (new MemoryStream (), parameters);
				success &= this.Source.TryProcess (sourceInteraction);

				if (sourceInteraction.HasWriter ())
					sourceInteraction.GetOutgoingBodyWriter ().Flush ();
				sourceInteraction.OutgoingBody.Position = 0;

				sourceReader = new StreamReader (sourceInteraction.OutgoingBody);
			}

			if (success) {
				string fileName = ComposeSinkFilename ();

				using (StreamWriter sinkWriter = new StreamWriter (fileName, this.Encoding)) {
					char[] buffer = new char[this.BufferSize];
					int position;

					while ((position = sourceReader.Read (buffer, 0, buffer.Length)) > 0)
						sinkWriter.Write (buffer, 0, position);
				}
			}

			return success;
		}
	}
}

