using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.ApolloGeese.Duckling.Http;
using System.Data;

namespace BorrehSoft.Extensions.BasicWeblings.Data
{
	public class ResultInteraction : QuickInteraction
	{
		public ResultInteraction (IInteraction Parent, IDataReader reader) : base(Parent)
		{
			for(int i = 0; i < reader.FieldCount; i++)
				this[reader.GetName(i)] = reader.GetValue(i);
		}

		public override string ToString ()
		{
			string[] keys = new string[base.Dictionary.Count];
			base.Dictionary.Keys.CopyTo(keys, 0);
			return string.Join(",", keys);
		}	
	}
}

