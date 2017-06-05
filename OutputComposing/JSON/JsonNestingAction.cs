using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Collections.Generic;
using BorrehSoft.Utilities.Collections.Settings;
using Commons.Json;
using JsonVal = Commons.Json.JsonValue;
using System.Web;
using BorrehSoft.Utilities.Log;
using Interactions;
using System.Collections.Specialized;

namespace BorrehSoft.ApolloGeese.Extensions.OutputComposing
{
	class JsonNestingAction : SimpleInteraction
	{
		public JsonArray Array { get; private set; }
		public Dictionary<string, JsonVal> Mapping { get; private set; }

		public JsonNestingAction (IInteraction parameters) : base(parameters)
		{
			this.Array = new JsonArray();
			this.Mapping = new Dictionary<string, JsonVal>();
		}

		public void Add(JsonVal value) 
		{
			this.Array.Add(value);
		}

		public void Map (string mapBackJsonVariable, JsonVal values)
		{
			Mapping.Add(mapBackJsonVariable, values);
		}
	}
}

