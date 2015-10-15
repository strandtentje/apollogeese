using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.IO;
using BorrehSoft.Utensils.Parsing;
using System.Text;

namespace InputProcessing
{
	/// <summary>
	/// Reluctant text reader.
	/// </summary>
	class ReluctantTextReader : TextReader
	{
		public TextReader Underlying {
			get;
			set;
		}

		public ReluctantTextReader (TextReader dataReader)
		{
			this.Underlying = dataReader;
		}

		public char StopCharacter { get; set; }

		public bool IsStopped {
			get {
				return this.Underlying.Peek () == StopCharacter;
			}
		}

		public override int Peek ()
		{
			if (IsStopped)
				return -1;
			else 
				return this.Underlying.Peek ();
		}

		public override int Read ()
		{
			if (IsStopped)
				return -1;
			else
				return this.Underlying.Read ();
		}

		public override string ReadLine ()
		{
			StringBuilder lineBuilder = new StringBuilder ();

			int character = Peek ();

			bool foundCr = false;
			bool foundLf = false;
			bool currentIsRegular = false;

			while (-1 < character) {

				currentIsRegular = false;
				if (character == 13)
					foundCr = true;
				else if (character == 10)
					foundLf = true;
				else 
					currentIsRegular = true;

				if ((foundCr || foundLf) && currentIsRegular) {
					return lineBuilder.ToString ();
				}

				lineBuilder.Append ((char)Read ());

				character = Peek ();
			}

			return lineBuilder.ToString ();
		}

		public override int Read (char[] buffer, int index, int count)
		{
			int character = Peek ();
			int writes = 0;

			while ((-1 < character) || (count > writes)) {
				buffer [index + writes++] = (char)Read ();
				character = Peek ();
			}

			return writes;
		}

		public override int ReadBlock (char[] buffer, int index, int count)
		{
			int readCharacters = Read (buffer, index, count);

			while (readCharacters < count) {			
				readCharacters = Read (buffer, index + readCharacters, count - readCharacters);
			}

			return readCharacters;
		}

		public override string ReadToEnd ()
		{
			int character = Peek ();

			StringBuilder fullBuilder = new StringBuilder ();

			while (-1 < character) {
				fullBuilder.Append ((char)Read ());
				character = Peek ();
			}

			return fullBuilder.ToString ();
		}
	}
}
