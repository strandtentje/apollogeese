using System;
using System.Text;

namespace BorrehSoft.Utensils
{
	public class HtmlTag
	{
		private Map<string> _attributes = new Map<string> ();
		private string _name;

		/// <summary>
		/// Gets the head.
		/// </summary>
		/// <value>The head.</value>
		public string Head { get; private set; }

		/// <summary>
		/// Gets the tail.
		/// </summary>
		/// <value>The tail.</value>
		public string Tail { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="BorrehSoft.Utensils.HtmlTag"/> doesnt have
		/// a closing element.
		/// </summary>
		/// <value><c>true</c> if dont close; otherwise, <c>false</c>.</value>
		public bool DontClose { get; private set; }

		public HtmlTag (string Name, bool DontClose)
		{
			this.Name = Name;
			this.DontClose = DontClose;
		}

		public HtmlTag (string Name)
		{
			this.Name = Name;
			this.DontClose = false;
		}
				
		/// <summary>
		/// Gets or sets the attributes.
		/// </summary>
		/// <value>The attributes.</value>
		public Map<string> Attributes 
		{
			get {
				return _attributes;
			}
			set {
				_attributes = value;
				Rerender ();
			}
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name {
			get { 
				return _name; 
			}
			set {
				_name = value;
				Rerender ();
			}
		}

		/// <summary>
		/// Rerender this HTMLtag's Head and Tail.
		/// </summary>
		public void Rerender ()
		{
			StringBuilder text = new StringBuilder ();

			text.Append ("<"); text.AppendLine (Name);

			Attributes.WritePairsTo (builder: text, format: " {0}=\"{1}\"");

			text.Append (">");

			Head = text.ToString ();
			Tail = DontClose ? "" : string.Format ("</{0}>", Name);
			
			text.Clear ();
		}
	}
}

