using System;
using BorrehSoft.Utensils.Collections.Settings;

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

		public string Serialize (Settings data)
		{
			throw new NotImplementedException ();
		}
	}
}

