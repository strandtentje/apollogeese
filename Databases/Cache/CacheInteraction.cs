using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Cache
{
	/// <summary>
	/// Cache interaction.
	/// </summary>
	class CacheInteraction : SimpleInteraction
	{
		/// <summary>
		/// The list for this Cache entity
		/// </summary>
		public List<IInteraction> List;

		/// <summary>
		/// Gets a value indicating whether this <see cref="BorrehSoft.ApolloGeese.Extensions.Data.Cache.CacheInteraction"/>
		/// requires filling
		/// </summary>
		/// <value><c>true</c> if requires fill; otherwise, <c>false</c>.</value>
		public bool RequiresFill { get; private set; }

		/// <summary>
		/// All caches by name
		/// </summary>
		public static Dictionary<string, List<IInteraction>> Lists = 
			new Dictionary<string, List<IInteraction>>();

		/// <summary>
		/// Gets or sets the name of this cachelist.
		/// </summary>
		/// <value>The name of the list.</value>
		public string ListName {
			get {
				return this.GetString ("listname");
			} set {
				this ["listname"] = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.ApolloGeese.Extensions.Data.Cache.CacheInteraction"/> class
		/// which gets listname from context.
		/// </summary>
		/// <param name="parameters">Parameters.</param>
		public CacheInteraction (IInteraction parameters) : base(parameters)
		{
			string listName;

			if (!parameters.TryGetFallbackString ("listname", out listName)) {
				throw new CacheException (
					"When not using listname setting, make sure" + 
					" listname is in context");
			}

			this.ListName = listName;
			SetCache ();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.ApolloGeese.Extensions.Data.Cache.CacheInteraction"/> class
		/// which gets listname defined explicitly.
		/// </summary>
		/// <param name="listName">List name.</param>
		/// <param name="parameters">Parameters.</param>
		public CacheInteraction (string listName, IInteraction parameters)
			: base(parameters)
		{
			this.ListName = listName;						
			SetCache ();
		}

		/// <summary>
		/// Either finds an existing fitting cache, or makes a new one and sets a
		/// flag indicating this cache is untouched.
		/// </summary>
		private void SetCache() {			
			if (Lists.ContainsKey (this.ListName)) {
				List = Lists [this.ListName];
				RequiresFill = false;
			} else {
				List = new List<IInteraction> ();
				RequiresFill = true;
			}
		}

		public void Clear() {
			List.Clear ();
		}

		/// <summary>
		/// Clear and dereference this list
		/// </summary>
		public void Purge() {
			Clear ();
			Lists.Remove (ListName);
		}
	}
}

