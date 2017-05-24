using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Collections.Generic;
using BorrehSoft.Utilities.Collections.Settings;
using Commons.Json;
using System.Web;
using BorrehSoft.Utilities.Log;
using Interactions;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	class JsonArrayInteraction : SimpleInteraction
	{
		public JsonArray Array { get; private set; }

		public JsonArrayInteraction (IInteraction parameters) : base(parameters)
		{
			this.Array = new JsonArray();
		}

		public void Add(Commons.Json.JsonValue value) 
		{
			this.Array.Add(value);
		}
	}
}

