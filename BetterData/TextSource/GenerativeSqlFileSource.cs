using System;
using BetterData;
using System.IO;

namespace BetterData
{
	abstract class GenerativeSqlFileSource : SqlFileSource
	{
		protected class GeneratedSqlException : Exception
		{
			public GeneratedSqlException (string message) : base(message)
			{

			}
		}

		public GenerativeSqlFileSource(string filePath) : base(filePath) {

		}

		protected abstract string Extension { get; }

		protected abstract void GenerateSqlForSections(string[] section, StreamWriter writer);

		public override string GetText ()
		{
			if (!File.Exists (BackEnd.FilePath)) {
				FileInfo info = new FileInfo (BackEnd.FilePath);

				using (StreamWriter writer = new StreamWriter(info.OpenWrite())) {
					string TinyInstruction = info.Name.Substring (
						0, info.Name.Length - Extension.Length);
					GenerateSqlForSections (TinyInstruction.Split(' '), writer);
				}
			}
			return base.GetText ();
		}
	}
}

