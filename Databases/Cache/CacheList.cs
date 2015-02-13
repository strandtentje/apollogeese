using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Maps;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;

namespace BorrehSoft.ApolloGeese.Extensions.Data.Cache
{
	/// <summary>
	/// Cache list.
	/// </summary>
	public class CacheList : Service
	{
		public override string Description {
			get {
				return string.Format ("get values from cached list {0}", this.listName);
			}
		}

		private bool relativePartition;
		private bool useConfigListname;
		private int partition;
		private Service unavailable;
		private Service iterator;
		private string listName;
		private string pageVariable { get; set; }

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "unavailable") this.unavailable = e.NewValue;
			if (e.Name == "iterator") this.iterator = e.NewValue;
		}

		protected override void Initialize (Settings modSettings)
		{
			Branches ["unavailable"] = Stub;

			this.useConfigListname = modSettings.TryGetString ("listname", out listName);
			this.partition = modSettings.GetInt ("partition");
			this.relativePartition = modSettings.GetBool ("relativepartition");
			this.pageVariable = modSettings.GetString ("pagevariable", "page");
		}

		bool ListPick (List<IInteraction> ourList, int i, IInteraction parameters)
		{			
			if (i < ourList.Count) {
				return iterator.TryProcess (ourList [i].Clone (parameters));
			}

			return true;
		}

		protected override bool Process (IInteraction parameters)
		{
			CacheInteraction cache;
			bool success = true;
			object pageObj; int page;

			if (useConfigListname) {
				cache = new CacheInteraction (this.listName, parameters);
			} else {
				cache = new CacheInteraction (parameters);
			}

			if (cache.RequiresFill)
				unavailable.TryProcess (cache);

			if (parameters.TryGetFallback (this.pageVariable, out pageObj)) 
				page = (int)pageObj;		
			else 
				throw new CacheException ("pagenumber missing");		

			if (relativePartition) {
				int pageSize = (int)Math.Ceiling ((float)cache.List.Count / (float)partition);
				for (int i = pageSize * page; i < (pageSize * (page + 1)); i++) 
					success &= ListPick (cache.List, i, parameters);

			} else {
				for (int i = this.partition * page; i < (this.partition * (page + 1)); i++) 
					success &= ListPick (cache.List, i, parameters);
			}

			return success;
		}
	}
}

