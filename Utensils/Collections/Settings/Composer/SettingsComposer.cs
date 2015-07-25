using System;
using BorrehSoft.Utensils.Collections.Settings;
using System.Text;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections;

namespace BorrehSoft.Utensils
{
	public class SettingsComposer
	{
		private int depth;
		private StringBuilder builder;
		private char startBlock;
		private char endBlock;
		private char entitySe;
		private char startArr;
		private char endArr;
		private char arrSe;
		private char couplerChar;


		public SettingsComposer (
			char startBlock = '{', char endBlock = '}', char entitySe = ';', 
			char startArr = '[', char endArr = ']', char arrSe = ',',
			char couplerChar = '=')
		{
			this.depth = 0;
			this.builder = new StringBuilder ();

			this.startBlock = startBlock;
			this.endBlock = endBlock;
			this.entitySe = entitySe;
			this.startArr = startArr;
			this.endArr = endArr;
			this.arrSe = arrSe;
			this.couplerChar = couplerChar;
		}

		public string Serialize (Settings data)
		{
			//start composing a Settings object
			builder.Append ('\t', depth);
			builder.Append (startBlock);
			builder.Append ("\n");
			depth++;

			//begin serializing internal information
			foreach(KeyValuePair<string, object> pair in data.Dictionary){

				builder.Append ('\t', depth);
				builder.Append (pair.Key);
				builder.Append (" ");
				builder.Append (couplerChar);
				builder.Append (" ");

				if (pair.Value is Settings) {
					//compose this (sub)settings data
				} else if (pair.Value is IEnumerable<string>) {
					//compose an array of strings
				} else if (pair.Value is String) {
					builder.Append ("\"");
					builder.Append (pair.Value);
					builder.Append ("\"");
				} else {
					builder.Append (pair.Value);
				}

				builder.Append (entitySe);
				builder.Append ("\n");

			}

			//close a Settings object
			depth--;
			builder.Append ('\t', depth);
			builder.Append (endBlock);

			return builder.ToString ();
		}
	}
}

