using System;
using System.IO;
using System.Collections.Generic;

namespace BorrehSoft.Utensils
{
	public class Row : List<string>
	{
		public Table Parent {
			get;
			private set;
		}

		public Row (Table table, IEnumerable<string> row) : base(row)
		{
			this.Parent = table;
		}

		public string this[string name] {
			get {
				return this [this.Parent.Header.IndexOf (name)];
			} set {
				int cellIndex;

				if (this.Parent.Header.Contains (name)) {
					cellIndex = this.Parent.Header.Contains (name);
				} else {
					cellIndex = this.Parent.Header.Count;
					this.Parent.Header.Add (name);
				}

				if (cellIndex < this.Count) {
					this.RemoveAt (cellIndex);
				}

				this.Insert (cellIndex, value);
			}
		}
	}
}

