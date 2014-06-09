using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.ApolloGeese.Duckling.Http;
using System.Data;

namespace BorrehSoft.Extensions.BasicWeblings
{
	public class ResultInteraction : Map<object>, IInteraction
	{
		public ResultInteraction (IInteraction Parent, IDataReader reader)
		{
			this.Parent = Parent;
			for(int i = 0; i < reader.FieldCount; i++)
				this[reader.GetName(i)] = reader.GetValue(i);
		}

		public IInteraction GetClosest (Type t)
		{
			for (IInteraction current = this; (current != null); current = current.Parent) {
				if (t.IsAssignableFrom(current.GetType())) return current;
			}

			throw new Exception("No interaction in chain was of specified type");
		}

		public override string ToString ()
		{
			string[] keys = new string[base.Dictionary.Count];
			base.Dictionary.Keys.CopyTo(keys, 0);
			return string.Join(",", keys);
		}

		public IInteraction Parent {
			get;
			private set;
		}

		public IInteraction Root {
			get { return this.Parent.Root; }
		}		
	}
}

