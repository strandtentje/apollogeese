using System.Text;
using System.Text.RegularExpressions;

namespace BorrehSoft.ApolloGeese.Duckling.Http.Headers
{
	/// <summary>
	/// MIME type.
	/// </summary>
	public class MimeType
	{
		public override string ToString ()
		{
			return PlainTextName + "; charset=" + Encoding.WebName;
		}

		public MimeType(string PlainTextName)
		{
			this.PlainTextName = PlainTextName;
		}

		/// <summary>
		/// Instantiates a new MimeType from string.
		/// </summary>
		/// <returns>The string.</returns>
		/// <param name="cType">C type.</param>
		public static MimeType FromString (string cType)
		{	
			string[] chunks = cType.Split(';');

			MimeType mt = new MimeType (chunks [0]);

			if (chunks.Length > 1) {
				mt.Encoding = Encoding.GetEncoding (Regex.Replace (
					chunks [1].Trim (),
					"charset=([A-Za-z0-9\\-]+)",
					"\\1"));
			}

			return mt;
		}

		/// <summary>
		/// Gets the encoder.
		/// </summary>
		/// <value>The encoder.</value>
		public Encoding Encoding { get; set; }

		/// <summary>
		/// Gets the plain text MIME type name.
		/// </summary>
		/// <value>The plain text MIME type name.</value>
		public string PlainTextName { get; private set; }

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="BorrehSoft.ApolloGeese.Duckling.MimeType"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="BorrehSoft.ApolloGeese.Duckling.MimeType"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="BorrehSoft.ApolloGeese.Duckling.MimeType"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals (object obj)
		{
			if (obj is MimeType)
				return ((MimeType)obj).PlainTextName == this.PlainTextName;
			else
				return false;
		}

		/// <summary>
		/// Asserts the similarity to the supplied instance.
		/// </summary>
		/// <param name="that">The other instance.</param>
		public void AssertSimilarityTo (MimeType that)
		{
			if (this.PlainTextName != that.PlainTextName) {
				throw new MimeTypeMismatchException (expectedType: that, actualType: this);
			}
		}

		/// <param name="a">The alpha component.</param>
		/// <param name="b">The blue component.</param>
		public static bool operator ==(MimeType a, MimeType b)
		{
			if (a == null)
				return false;
			if (b == null)
				return false;

			if (a.PlainTextName == b.PlainTextName)
				if (a.Encoding.WebName == b.Encoding.WebName)
					return true;

			return false;
		}

		/// <param name="a">The alpha component.</param>
		/// <param name="b">The blue component.</param>
		public static bool operator !=(MimeType a, MimeType b)
		{
			return !(a == b);
		}
	
		public static class Text 
		{
			/// <summary>
			/// Gets the text/html mimetype.
			/// </summary>
			/// <value>The html.</value>
			public static MimeType Html { get { return new MimeType ("text/html"); } }
		}	
	}
}

