using System;
using System.Drawing;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utensils.Collections.Settings;
using System.Collections.Generic;
using BorrehSoft.Utensils.Collections;
using BorrehSoft.Utensils.Collections.Maps;
using System.IO;
using System.Drawing.Imaging;
using BorrehSoft.Utensils.Log;
using System.Globalization;

namespace Imaging
{
	public class Scale : ImagingService
	{
		public override string Description {
			get {
				return "scale";
			}
		}

		public override void LoadDefaultParameters (string defaultParameter)
		{
			this.Settings ["defaultscale"] = float.Parse (defaultParameter, CultureInfo.InvariantCulture.NumberFormat);
		}

		float DefaultScale {
			get;
			set;
		}

		Map<float> Scales = new Map<float> ();

		IEnumerable<string> scaleSteps;

		IEnumerable<string> ScaleSteps {
			get {
				return scaleSteps;
			}
			set {
				scaleSteps = value;

				foreach (string step in value) {
					if (GetSettings().Has (step)) {
						object stepValue = GetSettings() [step];

						if (stepValue is float)
							Scales [step] = (float)stepValue;
						else if (stepValue is string)
							Scales [step] = float.Parse ((string)stepValue, CultureInfo.InvariantCulture.NumberFormat);
						else if (stepValue is int)
							Scales [step] = ((float)(int)stepValue) / 100f;
					}
				}
			}
		}

		string Stepvar {
			get;
			set;
		}

		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);
			this.DefaultScale = settings.GetFloat ("defaultscale", 1.0f);
			this.ScaleSteps = settings.GetStringList ("scalesteps", new string[] {});
			this.Stepvar = settings.GetString ("stepvariable", "scale");
		}

		protected override bool Process (IInteraction parameters)
		{
			string candidateStep;
			bool success = false;

			float scaling = DefaultScale;

			if (parameters.TryGetFallbackString (this.Stepvar, out candidateStep)) {
				if (Scales.Has (candidateStep)) {
					scaling = Scales [candidateStep];
				}
			}
						
			return TryGetImage (parameters, delegate(Image inImage) {
				int newWidth = (int)(((float)inImage.Width) * scaling);
				int newHeight = (int)(((float)inImage.Height) * scaling);

				return new Bitmap (
					inImage, 
					newWidth, 
					newHeight);
			});
		}
	}
}

