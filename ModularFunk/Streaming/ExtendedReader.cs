using System;
using System.IO;
using System.Text;

namespace BorrehSoft.Utensils
{
	/// <summary>
	/// Extended streamreader.
	/// </summary>
	public class ExtendedReader
	{
		public Stream BaseStream { get; private set; }

		public ExtendedReader (Stream underlying) 
		{
			this.BaseStream = underlying;
		}

		static readonly int LongSize = sizeof(long);
		static readonly int IntSize = sizeof(int);
		static readonly int ShortSize = sizeof(short);

		public char ReadChar ()
		{
			return (char)BaseStream.ReadByte();
		}

		public short ReadInt16 ()
		{
			byte[] data = new byte[ShortSize];
			BaseStream.Read(data, 0, ShortSize);
			return BitConverter.ToInt16(data, 0);
		}

		public int ReadInt32 ()
		{
			byte[] data = new byte[IntSize];
			BaseStream.Read(data, 0, IntSize);
			return BitConverter.ToInt32(data, 0);
		}

		/// <summary>
		/// Reads a long from the stream.
		/// </summary>
		/// <returns>
		/// The long.
		/// </returns>
		public long ReadInt64() 
		{
			byte[] data = new byte[LongSize];
			BaseStream.Read(data, 0, LongSize);
			return BitConverter.ToInt64(data, 0);
		}

		/// <summary>
		/// Reads specifiek amount of characters
		/// </summary>
		/// <returns>
		/// The characters.
		/// </returns>
		/// <param name='length'>
		/// Length.
		/// </param>
		public string ReadUntil (int length)
		{
			byte[] buf = new byte[length];
			this.BaseStream.Read(buf, 0, length);
			return BitConverter.ToString(buf);
		}

		/// <summary>
		/// Reads the until.
		/// </summary>
		/// <returns>
		/// The until.
		/// </returns>
		/// <param name='terminator'>
		/// Terminator.
		/// </param>
		public string ReadUntil (char terminator)
		{
			StringBuilder nString = new StringBuilder ();

			char inchar = (char)BaseStream.ReadByte();
			while (inchar != terminator) {
				nString.Append (inchar);
				inchar = (char)BaseStream.ReadByte();
			}

			return nString.ToString();
		}
	}
}

