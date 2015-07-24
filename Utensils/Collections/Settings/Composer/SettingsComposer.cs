using System;
using BorrehSoft.Utensils.Collections.Settings;
using System.IO;

namespace BorrehSoft.Utensils
{
	public class SettingsComposer
	{
		public SettingsComposer (
			char startBlock = '{', char endBlock = '}', char entitySe = ';', 
			char startArr = '[', char endArr = ']', char arrSe = ',',
			char couplerChar = '=')
		{

		}

		public void ToStreamWriter(Settings data, StreamWriter writer) {
			throw new NotImplementedException ();
		}

		public void ToStream(Settings data, Stream targetStream) {
			ToStreamWriter (data, new StreamWriter (targetStream));
		}

		public string Serialize (Settings data)
		{
			MemoryStream stream = new MemoryStream ();

			ToStream (data, stream);

			stream.Position = 0;

			StreamReader reader = new StreamReader (stream);

			return reader.ReadToEnd ();
		}
	}
}

