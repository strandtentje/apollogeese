using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;
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
				return string.Format ("get values from cached list {0}", this.ListName);
			}
		}
		
		private Service unavailable;
		private Service iterator;

		[Instruction("When set to true, partition size will be used as list count divider.")]
		public bool RelativePartition { get; set; }

		[Instruction("When set to true, uses listname-setting from configuration instead of context.")]
		public bool UseConfigListname { get; set; }

		[Instruction("When set to true, partitioning capabilitiers of CacheList will be unavailable.")]
		public bool DontPartition { get; set; }

		[Instruction("Size of partition. Is a divider or an absolute count based on RelativePartition setting.")]
		public int Partition { get; set; }

		[Instruction("List of the cache to use.")]
		private string ListName;

		[Instruction("Context variable to use for picking the correct partition page.")]
		public string pageVariable { get; set; }

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "unavailable") this.unavailable = e.NewValue;
			if (e.Name == "iterator") this.iterator = e.NewValue;
		}

		protected override void Initialize (Settings modSettings)
		{
			Branches ["unavailable"] = Stub;

			string listName;
			this.UseConfigListname = modSettings.TryGetString ("listname", out listName);
			this.ListName = listName;
			this.DontPartition = modSettings.GetBool ("dontpartition", false);

			if (!this.DontPartition) {
				this.Partition = modSettings.GetInt ("partition", 1);
				this.RelativePartition = modSettings.GetBool ("relativepartition");
				this.pageVariable = modSettings.GetString ("pagevariable");
			}
		}

		/// <summary>
		/// Pick from list
		/// </summary>
		/// <returns><c>true</c>, if successful, <c>false</c> otherwise.</returns>
		/// <param name="ourList">Our list.</param>
		/// <param name="i">The index.</param>
		/// <param name="parameters">Parameters.</param>
		bool ListPick (List<IInteraction> ourList, int i, IInteraction parameters)
		{			
			if (i < ourList.Count) {
				return iterator.TryProcess (ourList [i].Clone (parameters));
			}

			return true;
		}

		/// <summary>
		/// Iterates with partitioning
		/// </summary>
		/// <returns><c>true</c>, if successful, <c>false</c> otherwise.</returns>
		/// <param name="parameters">Parameters.</param>
		/// <param name="cache">Cache.</param>
		bool IterateWithPartition(IInteraction parameters, CacheInteraction cache) {
			bool success = true;
			object pageObj; int page;
			
			if (parameters.TryGetFallback (this.pageVariable, out pageObj)) 
				page = (int)pageObj;
			else 
				throw new CacheException ("pagenumber missing");		

			if (RelativePartition) {
				int pageSize = (int)Math.Ceiling ((float)cache.List.Count / (float)Partition);
				for (int i = pageSize * page; i < (pageSize * (page + 1)); i++) 
					success &= ListPick (cache.List, i, parameters);

			} else {
				for (int i = this.Partition * page; i < (this.Partition * (page + 1)); i++) 
					success &= ListPick (cache.List, i, parameters);
			}

			return success;
		}

		/// <summary>
		/// Iterates without partitioning.
		/// </summary>
		/// <returns><c>true</c>, if successful, <c>false</c> otherwise.</returns>
		/// <param name="parameters">Parameters.</param>
		/// <param name="cache">Cache.</param>
		bool IterateWithoutPartition(IInteraction parameters, CacheInteraction cache) {
			bool success = true;

			foreach (IInteraction item in cache.List)
				success &= iterator.TryProcess (item.Clone (parameters));

			return success;
		}

		protected override bool Process (IInteraction parameters)
		{
			CacheInteraction cache;

			if (UseConfigListname) {
				cache = new CacheInteraction (this.ListName, parameters);
			} else {
				cache = new CacheInteraction (parameters);
			}

			if (cache.RequiresFill)
				unavailable.TryProcess (cache);

			if (DontPartition) {
				return IterateWithoutPartition (parameters, cache);
			} else {
				return IterateWithPartition (parameters, cache);
			}
		}
	}
}

