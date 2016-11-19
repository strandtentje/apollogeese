using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections.Maps;
using System.Text;

namespace BorrehSoft.ApolloGeese.Extensions.Navigation
{
	/// <summary>
	/// Subsection interaction, produced after program flow was altered by URL
	/// </summary>
	class RouteInteraction : SimpleInteraction
	{
		/// <summary>
		/// Gets the parent http interaction
		/// </summary>
		/// <value>The parent http interaction.</value>
		public IHttpInteraction ParentHttp { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this instance has more 'directory'-names
		/// </summary>
		/// <value><c>true</c> if this instance has more 'directory'-names; otherwise, <c>false</c>.</value>
		public bool HasTail { get; private set; }

		public string ResourceNameKey { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BorrehSoft.Extensions.Navigation.SubsectionInteraction"/> class.
		/// </summary>
		/// <param name="http">Http.</param>
		/// <param name="parent">Parent.</param>
		public RouteInteraction(IHttpInteraction http, IInteraction parent, string resourceNameKey) : base(parent)
		{
			this.ParentHttp = http;
			this.ResourceNameKey = resourceNameKey;

			this [this.ResourceNameKey] = "main";
			// Directoryname is legacy!
			this ["directoryname"] = "main";

			HasTail = http.URL.Count > 0;

			if (HasTail) {
				this [this.ResourceNameKey] = http.URL.Peek ();
				this ["directoryname"] = http.URL.Peek();
			}
		}

		/// <summary>
		/// The branchname that was elected by the url section
		/// </summary>
		/// <value>The name of the branch.</value>
		public string DirectoryName {
			get {
				return (string)this [this.ResourceNameKey];
			}
		}

		/// <summary>
		/// Confirm this interaction was in correspondence with the given url.
		/// </summary>
		public void Confirm ()
		{
			if (HasTail) {
				ParentHttp.URL.Dequeue ();
			}

			if (ParentHttp.URL.Count > 0) {
				this ["nexturl"] = ParentHttp.URL.Peek ();
				this ["remainingurl"] = string.Join ("/", ParentHttp.URL.ToArray());
			}
		}
	}
}

