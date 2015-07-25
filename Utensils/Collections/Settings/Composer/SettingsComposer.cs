using System;
using BorrehSoft.Utensils.Collections.Settings;
using System.Text;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections;
using System.IO;

namespace BorrehSoft.Utensils
{
	public class SettingsComposer
	{
		private int depth;

		private char startBlock;
		private char endBlock;
		private char entitySe;
		private char startArr;
		private char endArr;
		private char arrSe;
		private char couplerChar;

		private bool first;


		public SettingsComposer (
			char startBlock = '{', char endBlock = '}', char entitySe = ';', 
			char startArr = '[', char endArr = ']', char arrSe = ',',
			char couplerChar = '=')
		{
			this.depth = 0;

			this.startBlock = startBlock;
			this.endBlock = endBlock;
			this.entitySe = entitySe;
			this.startArr = startArr;
			this.endArr = endArr;
			this.arrSe = arrSe;
			this.couplerChar = couplerChar;

			this.first = true;
		}

		public void ToStreamWriter(Settings data, StreamWriter writer) {

			writer.AutoFlush = true;

			//start composing a Settings object
			writer.Write (startBlock);
			depth++;

			//begin serializing internal information
			foreach(KeyValuePair<string, object> pair in data.Dictionary){

				if (first) {
					writer.Write ("\n");
					first = false;
				}
				writer.Write (new String ('\t', depth));
				writer.Write (pair.Key);
				writer.Write (" ");
				writer.Write (couplerChar);
				writer.Write (" ");

				if (pair.Value is Settings) {
					ToStreamWriter ((Settings)pair.Value, writer);
					//compose this (sub)settings data
				} else if (pair.Value is IEnumerable<object>) {
					writer.Write (startArr);
					//compose an array of strings
					writer.Write (endArr);
				} else if (pair.Value is String) {
					writer.Write ("\"");
					writer.Write (pair.Value);
					writer.Write ("\"");
				} else {
					writer.Write (pair.Value);
				}

				writer.Write (entitySe);
				writer.Write ("\n");

			}

			//close a Settings object
			depth--;
			writer.Write (new String ('\t', depth));
			writer.Write(endBlock);
			if (first) {
				writer.Write ("\n");
			}

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

