using System;
using BorrehSoft.ApolloGeese.Duckling;
using System.Diagnostics;
using DProcess = System.Diagnostics.Process;
using System.IO;
using BorrehSoft.Utensils.Collections;

namespace Testing.Diff
{
	/// <summary>
	/// Diff session.
	/// </summary>
	public class DiffSession
	{
		/// <summary>
		/// The diff tool process.
		/// </summary>
		Process diffToolProcess;

		/// <summary>
		/// Gets the diff tool command.
		/// </summary>
		/// <value>The diff tool command.</value>
		public string DiffToolCommand {
			get;
			private set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Testing.Diff.DiffSession"/> class.
		/// </summary>
		public DiffSession()
		{
			this.DiffToolCommand = "diff";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Testing.Diff.DiffSession"/> class.
		/// </summary>
		/// <param name="command">Command.</param>
		public DiffSession (string command)
		{			
			this.DiffToolCommand = command;
		}
			
		/// <summary>
		/// Sets the diff input.
		/// </summary>
		/// <param name="file">File.</param>
		/// <param name="incomingBody">Incoming body.</param>
		public void SetInput (string file, Stream incomingBody)
		{
			diffToolProcess = DProcess.Start (GetDiffToolStartInfo(file));

			incomingBody.CopyTo (diffToolProcess.StandardInput.BaseStream);
			diffToolProcess.StandardInput.Close ();
		}

		/// <summary>
		/// Gets the diff tool start info.
		/// </summary>
		/// <returns>The diff tool start info.</returns>
		ProcessStartInfo GetDiffToolStartInfo (string file)
		{
			return new ProcessStartInfo (
				this.DiffToolCommand, string.Format ("-u {0} -", file)) {
				UseShellExecute = false, RedirectStandardInput = true, RedirectStandardOutput = true
			};
		}

		/// <summary>
		/// Gets the diff output reader.
		/// </summary>
		/// <returns>The output reader.</returns>
		public StreamReader GetOutputReader ()
		{
			return diffToolProcess.StandardOutput;
		}

		public void Dispose ()
		{
			diffToolProcess.Dispose ();
		}
	}
}

