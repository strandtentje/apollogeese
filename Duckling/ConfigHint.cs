using System;
using System.Collections.Generic;
using System.Net;
using BorrehSoft.Utensils;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Log;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Collections.Maps;
using System.Text;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	public class ConfigHint
	{
		public ConfigHintType Type {
			get;
			private set;
		}

		public string Name {
			get;
			private set;
		}

		public string Explanation {
			get;
			private set;
		}

		public enum ConfigHintType : int {
			Numeric, Boolean, String, File, Array
		}

		public ConfigHint(ConfigHintType type, string name, string explanation = "") {
			this.Type = type;
			this.Name = name;
			this.Explanation = explanation;
		}
	}
}
