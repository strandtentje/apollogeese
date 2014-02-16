using System;

namespace BorrehSoft.ApolloGeese.Duckling.HTML
{
	public enum HtmlCapsuleType { 
		/// <summary>
		/// Hidden element.
		/// </summary>
		Hidden,
		/// <summary>
		/// No tags, only inner text.
		/// </summary>
		Bare, 
		/// <summary>
		/// Only opening tag. No closer. No inner text.
		/// </summary>
		OpenerOnly, 
		/// <summary>
		/// Only opening tag with a slash near the terminating >
		/// </summary>
		ClosingOpener, 
		/// <summary>
		/// Inner text encapsuled by an opening tag and a closing tag
		/// </summary>
		Capsule 
	}
}

