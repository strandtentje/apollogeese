using System;
using BorrehSoft.Utensils.Collections;
using System.Reflection;

namespace BorrehSoft.Utensils.Colections.Maps
{
	public class ObjectMap : Map<object>
	{
		Type baseType;
		object baseObject;

		public ObjectMap(object baseObject)
		{
			this.baseObject = baseObject;
			this.baseType = baseObject.GetType();
		}

		public override bool Has (string key)
		{
			PropertyInfo property = baseType.GetProperty (key);
			if (property != null) {
				return true;
			} else {
				return base.Has(key);
			}
		}

		public override object Get (string key)
		{
			PropertyInfo property = baseType.GetProperty (key, BindingFlags.IgnoreCase);
			if (property != null) {
				return property.GetValue (baseObject, null);
			} else {
				return base.Get(key);
			}
		}
	}
}

