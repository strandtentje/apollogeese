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
using System.Text;

namespace Imaging
{
	public abstract class ImagingService : SingleBranchService
	{		
		ImageFormat selectedImageformat;

		protected static Bitmap emptyBitmap = new Bitmap(1,1);
			
		bool UseJpgInsteadOfPng {
			get { return selectedImageformat == ImageFormat.Jpeg; }
			set {
				selectedImageformat = ImageFormat.Png;
			}
		}

		protected override void Initialize (Settings settings)
		{
			base.Initialize (settings);
			this.UseJpgInsteadOfPng = settings.GetBool ("usejpginsteadofpng", true);
		}

		protected delegate Bitmap ImageProcessor (Image inImage);

		protected bool TryGetImage(IInteraction parameters, ImageProcessor imageCallback) {
			bool success = false;

			IInteraction candidateImageOut;
			if (parameters.TryGetClosest (typeof(IOutgoingBodiedInteraction), out candidateImageOut)) {
				IOutgoingBodiedInteraction imageOut = (IOutgoingBodiedInteraction)candidateImageOut;

				if (imageOut is IHttpInteraction) {
					if (this.UseJpgInsteadOfPng) {
						((IHttpInteraction)imageOut).ResponseHeaders["Content-Type"] = "image/jpg";
					} else {
						((IHttpInteraction)imageOut).ResponseHeaders["Content-Type"] = "image/png";
					}
				}

				MemoryStream imageData = new MemoryStream ();
				SimpleOutgoingInteraction imageSourcer = new SimpleOutgoingInteraction (
					imageData, Encoding.Default, parameters);

				if (WithBranch.TryProcess (imageSourcer)) {
					Image inImage = Bitmap.FromStream (imageData);

					Bitmap outImage = imageCallback (inImage);

					inImage.Dispose ();

					if (outImage != emptyBitmap) {	
						outImage.Save (imageOut.OutgoingBody, selectedImageformat);

						outImage.Dispose ();
					}

					success = true;
				} else {
					Secretary.Report (5, "Image source failure");
				}
			} else {
				Secretary.Report (5, "No outgoing body found to write result image to.");
			}

			return success;
		}

	}
}

