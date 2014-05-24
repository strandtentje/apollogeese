using System;
using System.IO;
using System.Collections.Generic;

namespace BorrehSoft.Utensils.Collections.Maps
{
	public class StreamingMap : Map<object>
	{		
		const char 
			MapOpener = '\n',
			MapTerminator = '\r',
			IdentTerminator = '=',
			DecimalIndicator = 'd',
			UnicodeIndicator = 'u';

		/// <summary>
		/// Fills map from stream
		/// </summary>
		/// <param name='input'>
		/// Input.
		/// </param>
		public static StreamingMap FromStream(Stream input)
		{
			StreamingMap streamData = new StreamingMap();

			ExtendedReader extendedReader = new ExtendedReader(input);

			string ident; int length;
			char type = extendedReader.ReadChar();

			while (type != MapOpener)
				type = extendedReader.ReadChar();

			type = extendedReader.ReadChar();

			while (type != MapTerminator) {
				ident = extendedReader.ReadUntil(IdentTerminator);

				switch(type)
				{
				case DecimalIndicator:
					streamData[ident] = extendedReader.ReadInt64(); break;
				case UnicodeIndicator:
					length = extendedReader.ReadInt32();
					streamData[ident] = extendedReader.ReadUntil(length); break;		
				default: break;
				}

				type = extendedReader.ReadChar();
			}

			return streamData;
		}

		/// <summary>
		/// Puts map to stream
		/// </summary>
		/// <param name='output'>
		/// Output.
		/// </param>
		public void ToStream (Stream output)
		{
			ExtendedWriter extendedWriter = new ExtendedWriter(output);

			string sIdent; long value; string sValue;

			extendedWriter.WriteChar(MapOpener);

			foreach(KeyValuePair<string, object> kvp in this.Dictionary)
			{
				sIdent = kvp.Key;

				if (value.GetType() == typeof(long))
				{
					value = (long)kvp.Value;

					extendedWriter.WriteChar(DecimalIndicator);	
					extendedWriter.WriteString(sIdent);
					extendedWriter.WriteChar(IdentTerminator);
					extendedWriter.WriteLong(value);
				} else {
					sValue = kvp.Value.ToString();

					extendedWriter.WriteChar(UnicodeIndicator);
					extendedWriter.WriteString(sIdent);
					extendedWriter.WriteChar(IdentTerminator);
					extendedWriter.WriteInt(sValue.Length);
					extendedWriter.WriteString(sValue);
				}
			}

			extendedWriter.WriteChar(MapTerminator);
		}

		public long GetLong (string name, int dflt)
		{
			if (this[name] == null) return dflt;
			if (this[name].GetType() != typeof(long)) return dflt;

			return (long)this[name];
		}
	}
}

