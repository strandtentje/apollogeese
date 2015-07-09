using System;
using System.Diagnostics;
using DProcess = System.Diagnostics.Process;
using System.IO;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;

namespace Testing
{
	class FileDiffInteraction : IIncomingBodiedInteraction, IDisposable
	{
		Process diffToolProcess;

		public FileDiffInteraction(string diffToolCommand, string leftFile, Stream rightStream) {
			ProcessStartInfo diffToolStartinfo = new ProcessStartInfo (
				diffToolCommand,
				string.Format ("{0} -", leftFile));

			diffToolStartinfo.UseShellExecute = false;
			diffToolStartinfo.RedirectStandardInput = true;
			diffToolStartinfo.RedirectStandardOutput = true;

			diffToolProcess = Process.Start (diffToolStartinfo);

			rightStream.CopyTo (diffToolProcess.StandardInput.BaseStream);

			this.IncomingBody = diffToolProcess.StandardOutput;
		}

		public void Dispose() {
			this.diffToolProcess.Dispose ();
		}

		Stream IncomingBody { get; }

		StreamReader reader;

		StreamReader GetIncomingBodyReader() {
			if (!HasReader) {
				reader = new StreamReader (IncomingBody);
			}

			return reader;
		}

		/// <summary>
		/// Gets a value indicating whether or not a reader has been produced for the stream
		/// </summary>
		/// <returns><c>true</c> if this instance has a reader; otherwise, <c>false</c>.</returns>
		bool HasReader() {
			return reader != null;
		}
	}
}

