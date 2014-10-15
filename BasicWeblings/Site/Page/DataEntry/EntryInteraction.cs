using System;
using BorrehSoft.ApolloGeese.Duckling.Http;
using System.IO;
using BorrehSoft.ApolloGeese.Duckling;
using System.Text;
using BorrehSoft.Utensils.Collections;

namespace BorrehSoft.Extensions.BasicWeblings.Site.Page.DataEntry
{
	public class EntryInteraction : QuickInteraction
	{		
		public event InputAcceptedEventHandler InputAccepted;
		public event FormDisplayingEventHandler FormDisplaying;

		public EntryInteraction (IInteraction Parent, Map<object> Values) : base(Parent, Values)
		{
		
		}

		public void RaiseInputAccepted (IInteraction parameters)
		{
			if (InputAccepted == null)
				return;

			InputAccepted (this, new InputAcceptedEventArgs (parameters));
		}

		public void RaiseFormDisplaying (StreamWriter Writer, bool EntryAttempt)
		{
			if (FormDisplaying == null)
				return;

			FormDisplaying (this, new FormDisplayingEventArgs (Writer, EntryAttempt));
		}
	}
}

