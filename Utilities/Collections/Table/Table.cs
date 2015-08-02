using System;
using System.IO;
using System.Collections.Generic;

namespace BorrehSoft.Utensils
{
	public class Table : List<Row>
	{
		public Table ()
		{
		}

		public Header Header {
			get;
			private set;
		}

		public void SetHeader(params string[] header) {
			SetHeader (header);
		}

		public void SetHeader(IEnumerable<string> header) {
			this.Header = new Header (this, header);
		}

		public void Add(params string[] row) {
			Add (row);

		}
		public void Add(IEnumerable<string> row) {
			this.Add (new Row (this, row));
		}

		public static Table FromCSV(Stream source, char comma = ',', char encloser = '"') {
			Table newTable = new Table ();

			using (StreamReader reader = new StreamReader(source)) {
				var data = CsvParser.ParseHeadAndTail (reader, comma, encloser);
				// fine. i'll dynamically type this.
				// just know i'm not happy about this at all.

				newTable.SetHeader (data.Item1);

				foreach (IEnumerable<string> line in data.Item2) {
					newTable.Add (line);
				}
			}

			return newTable;
		}
	}
}

