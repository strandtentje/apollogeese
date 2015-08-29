using System;
using BorrehSoft.Utensils.Collections.Settings;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Drawing;

namespace Imaging
{
	public class Tile : GenericCrop
	{
		int Width {
			get;
			set;
		}

		int Height {
			get;
			set;
		}

		string XVariable {
			get;
			set;
		}

		string YVariable {
			get;
			set;
		}

		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);

			this.Width = settings.GetInt ("width");
			this.Height = settings.GetInt ("height");

			this.XVariable = settings.GetString ("xvariable", "left");
			this.YVariable = settings.GetString ("yvariable", "top");
		}

		protected override Rectangle GetBounds (IInteraction parameters)
		{		
			Rectangle bounds = Rectangle.Empty;
			object xCandidate, yCandidate;

			if (parameters.TryGetFallback (this.XVariable, out xCandidate) && 
			    parameters.TryGetFallback (this.YVariable, out yCandidate)) {

				int xPos = 0, yPos = 0;

				if (xCandidate is int)
					xPos = (int)xCandidate;
				else if (xCandidate is string)
					xPos = int.Parse ((string)xCandidate);

				if (yCandidate is int)
					yPos = (int)yCandidate;
				else if (yCandidate is string)
					yPos = int.Parse ((string)yCandidate);


				bounds = new Rectangle (
					new Point (xPos * this.Width, yPos * this.Height),
					new Size (this.Width, this.Height));
			}

			return bounds;
		}

	}
}

