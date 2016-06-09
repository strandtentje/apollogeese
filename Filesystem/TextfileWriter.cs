using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Text;
using BorrehSoft.Utensils.Log;

namespace Filesystem
{
	public class TextfileWriter : SinkService
	{
		public int BufferSize { get; private set; }

		public string DirectoryPath { get; private set; }

		public string Filename { get; private set; }

		public string FilenameVariable { get; private set; }

		public bool HasGeneratedFilename { get { return Filename.Length == 0; } }

		public bool AppendExisting { get; private set; }

		private Service Success = Stub;

		public override string Description {
			get {
				string description = "file writer";

				try {
					if (this.HasGeneratedFilename) {
						description = string.Format ("Writes uniquely named file to directory {0}",
							this.DirectoryPath);
					} else {
						description = string.Format ("Writes file to directory {0}, using name {1}",
							this.DirectoryPath, this.Filename);
					}
				} catch (Exception ex) {
					// ignore!
				}

				return description;
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			if (File.Exists (defaultParameter)) {
				FileInfo info = new FileInfo (defaultParameter);

				if (info.Exists) {
					this.Settings["rootpath"] = info.DirectoryName;
					this.Settings["filename"] = info.Name;
				}
			} else if (Directory.Exists (defaultParameter)) {
				this.Settings["rootpath"] = defaultParameter;
				this.Settings ["filename"] = "";
			}
		}

		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);
			this.DirectoryPath = settings.GetString ("rootpath");
			this.Filename = settings.GetString ("filename", "");
			this.AppendExisting = settings.GetBool ("append", false);
			this.BufferSize = settings.GetInt ("buffersize", 1024 * 128);
			this.FilenameVariable = settings.GetString ("filenamevariable", "filename");
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

		private void WriteFileFromReader(TextReader sourceReader, Encoding encoding, string fileName) {
			using (StreamWriter sinkWriter = new StreamWriter (fileName, this.AppendExisting, encoding)) {
				char[] buffer = new char[this.BufferSize];
				int position;

				while ((position = sourceReader.Read (buffer, 0, buffer.Length)) > 0)
					sinkWriter.Write (buffer, 0, position);
			}
		}

		protected override bool Process (IInteraction parameters)
		{
			bool successful = true;

			try {
				string fileName = ComposeSinkFilename ();
				WriteFileFromReader(
					GetReader(parameters), 
					GetEncoding(parameters), 
					fileName
				);
				successful &= this.Success.TryProcess(
					new SimpleInteraction(
						parameters, 
						this.FilenameVariable, 
						fileName
					)
				);
			} catch(Exception ex) {
				Secretary.Report (5, ex.Message);
				successful &= this.FailForException (parameters, ex);
			}
				
			return successful;
		}
	}
}

