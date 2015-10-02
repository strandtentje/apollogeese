using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Maps;
using System.Data;
using System.Collections.Generic;

namespace BetterData
{
	public class BranchesByNumber 
	{
		public BranchesByNumber(string prefix) {
			this.BranchPrefix = prefix;
		}

		public string BranchPrefix { get; private set; }

		Service[] numberedBranches = new Service[0];

		public void SetBranch (string name, string service)
		{
			if (name.StartsWith (BranchPrefix)) {
				int branchNumber;
				if (int.TryParse (name.Substring (BranchPrefix.Length), out branchNumber)) {
					if (numberedBranches.Length <= branchNumber) {
						Array.Resize (ref numberedBranches, branchNumber + 1);
					}

					numberedBranches [branchNumber] = service;
				}
			}
		}

		public Service DefaultBranch = Service.Stub;

		public Service Find (int branchNumber)
		{
			try {
				return numberedBranches[branchNumber] ?? this.DefaultBranch;
			} catch (IndexOutOfRangeException ex) {
				// yolo
				return Service.Stub;
			}
		}
	}


}

