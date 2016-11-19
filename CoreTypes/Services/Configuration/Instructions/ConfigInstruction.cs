using System;
using System.Collections.Generic;
using System.Net;
using BorrehSoft.Utilities;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Log;
using BorrehSoft.Utilities.Collections;
using BorrehSoft.Utilities.Collections.Maps;
using System.Text;

namespace BorrehSoft.ApolloGeese.CoreTypes
{
	public class ConfigInstruction
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

		public object CurrentValue {
			get;
			set;
		}

		public enum ConfigHintType : int {
			Numeric, Boolean, String, File, Array, Options
		}

		public ConfigInstruction(ConfigHintType type, string name, string explanation = "") {
			this.Type = type;
			this.Name = name;
			this.Explanation = explanation;
		}
	}
}
