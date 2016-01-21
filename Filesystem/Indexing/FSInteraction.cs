using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.Utensils.Collections.Maps;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Collections.Concurrent;
using BorrehSoft.ApolloGeese.Extensions.Filesystem;

namespace Filesystem
{
	class FSInteraction : IInteraction
	{
		public FileSystemInfo Info {
			get;
			private set;
		}

		public string RootPath { get; private set; }

		public string LastDate {
			get {
				return this.Info.LastWriteTime.ToString (
					"s", System.Globalization.CultureInfo.InvariantCulture);
			}
		}

		public string URL {
			get {				
				return Info.FullName.Remove (0, this.RootPath.Length);
			}
		}

		public string ParentURL {
			get {
				return URL.Remove (URL.Length - this.Info.Name.Length);
			}
		}

		public string Type {
			get {
				if (this.Info is DirectoryInfo) 
					return "directory";
				else if (this.Info is FileInfo) 
					return "file";
				else 
					return "unknown";
			}
		}

		public FSInteraction(FileSystemInfo info, string rootPath, IInteraction parameters = null) {			
			this.Info = info;	
			this.RootPath = rootPath;
			this.Parent = parameters;
			this.ExceptionHandler = this.Parent.ExceptionHandler;
		}

		public ExceptionHandler ExceptionHandler { get; private set; }

		public IInteraction Root { get { return (Parent == null ? null : Parent.Root); } }

		public IInteraction Parent { get; private set; }

        public bool TryGetClosest(Type t, IInteraction limit, out IInteraction closest)
        {
            for (closest = this; (closest != limit); closest = closest.Parent)
                if (t.IsAssignableFrom(closest.GetType()))
                    return true;

            return false;
        }

        public bool TryGetClosest(Type t, out IInteraction closest)
        {
            return TryGetClosest(t, null, out closest);
        }

		public IInteraction GetClosest (Type t)
		{
			IInteraction closest;

			if (t == null)
				throw new ArgumentNullException ("Type required for GetClosest");

			if (!TryGetClosest(t, out closest))
				throw new Exception(string.Format("No interaction in chain was of type", t.Name));

			return closest;
		}

		public IInteraction Clone(IInteraction parent) {
			throw new UnclonableException ();
		}

		public bool TryGetString(string id, out string luggage) {
			object value; luggage = null;
			if (TryGetValue (id, out value)) {
				luggage = value.ToString ();
				return true;
			}
			return false;
		}

		bool TryGetPropValue(object obj, string name, out object value) {
			value = null;

			try {
				PropertyInfo info = obj.GetType ().GetProperty (name, 
				                                                BindingFlags.IgnoreCase | 
				                                                BindingFlags.Public | 
				                                                BindingFlags.Instance);
				value = info.GetValue(obj, null);
				return true;
			} catch(Exception ex) {
				return false;
			}
		}

		public bool TryGetValue(string id, out object luggage) {		
			return TryGetPropValue (this.Info, id, out luggage) || TryGetPropValue (this, id, out luggage);
		}

		public object this [string name] { 
			get {
				object value;
				TryGetValue (name, out value);
				return value;
			}
		}

		public bool TryGetFallbackString(string id, out string luggage) {
			return TryGetString (id, out luggage) || ((this.Parent != null) && this.Parent.TryGetFallbackString (id, out luggage));
		}

		public bool TryGetFallback (string id, out object luggage) {
			return TryGetValue (id, out luggage) || ((this.Parent != null) && this.Parent.TryGetFallback (id, out luggage));
		}
	}
}

