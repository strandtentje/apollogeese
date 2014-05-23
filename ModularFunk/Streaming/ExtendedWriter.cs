using System;
using System.IO;
using System.Text;

namespace BorrehSoft.Utensils
{
	public class ExtendedWriter
	{
		public Stream BaseStream { get; private set; }

		public ExtendedWriter (Stream output)
		{
			this.BaseStream = output;
		}

		public void WriteChar (char chr)
		{
			WriteByte((byte)chr);
		}

		public void WriteBytes(byte[] bytes)
		{
			BaseStream.Write(bytes, 0, bytes.Length);
		}

		public void WriteByte(byte value)
		{
			BaseStream.WriteByte(value);
		}

		public void WriteShort (short value)
		{
			WriteBytes(BitConverter.GetBytes(value));
		}

		public void WriteLong(long value)
		{
			WriteBytes(BitConverter.GetBytes(value));
		}

		public void WriteInt(int value)
		{
			WriteBytes(BitConverter.GetBytes(value));
		}

		public void WriteString (string data)
		{
			WriteBytes(Encoding.ASCII.GetBytes(data));
		}

	}
}

