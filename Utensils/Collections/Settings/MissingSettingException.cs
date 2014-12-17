using System;

namespace BorrehSoft.Utensils.Collections.Settings
{
	public class MissingSettingException : Exception
	{
		public MissingSettingException (string section, string settingName, string type) : 
			base(string.Format("In the the section {0}, {1} with the name \"{2}\" was expected.", 
			                   section, type, settingName)) { }
	}
}

