using System;
using BorrehSoft.ApolloGeese.Duckling.Http;
using System.IO;
using BorrehSoft.BorrehSoft.Utensils.Collections;
using BorrehSoft.ApolloGeese.Duckling;
using System.Text;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Page.DataEntry
{
	public class EntryInteraction : Map<object>, IInteraction
	{		
		public event InputAcceptedEventHandler InputAccepted;
		public event FormDisplayingEventHandler FormDisplaying;

		public Map<string> Values { get; private set; }

		public EntryInteraction (Map<string> Values)
		{
			this.Values = Values;
		}

		public void RaiseInputAccepted (IInteraction parameters)
		{
			if (InputAccepted == null)
				return;

			InputAccepted (this, new InputAcceptedEventArgs (parameters, this.Values));
		}

		public void RaiseFormDisplaying (StreamWriter Writer, bool EntryAttempt)
		{
			if (FormDisplaying == null)
				return;

			FormDisplaying (this, new FormDisplayingEventArgs (Writer, EntryAttempt, this.Values));
		}
	}
}

