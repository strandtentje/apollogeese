using System;
using BorrehSoft.ApolloGeese.Duckling;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.Http;
using BorrehSoft.Utensils.Collections.Maps;

namespace BorrehSoft.Extensions.BasicWeblings.Site
{
	class SubsectionInteraction : QuickInteraction
	{
		public IHttpInteraction ParentHttp { get; private set; }
		public bool HasTail { get; private set; }

		public SubsectionInteraction(IHttpInteraction http, IInteraction parent) : base(parent)
		{
			this.ParentHttp = http;

			this ["branchname"] = "main";

			HasTail = http.URL.Count > 0;

			if (HasTail) 
				this ["branchname"] = http.URL.Peek ();

		}

		public string BranchName {
			get {
				return (string)this ["branchname"];
			}
		}

		public void Confirm ()
		{
			if (HasTail) {
				ParentHttp.URL.Dequeue ();
			}

			if (ParentHttp.URL.Count > 0) {
				this ["nexturl"] = ParentHttp.URL.Peek ();
			}
		}
	}
}

