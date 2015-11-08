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
using BorrehSoft.ApolloGeese.Http;
using BorrehSoft.ApolloGeese.Http.Headers;
using System.Text;

namespace Imaging
{
	public abstract class ImagingService : Service
	{		
		Service Source;

		ImageFormat selectedImageformat;

		protected static Bitmap emptyBitmap = new Bitmap(1,1);

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			if (e.Name == "source") 
				this.Source = e.NewValue;
		}

		bool UseJpgInsteadOfPng {
			get { return selectedImageformat == ImageFormat.Jpeg; }
			set {
				selectedImageformat = ImageFormat.Png;
			}
		}

		protected override void Initialize (Settings settings)
		{
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
						((IHttpInteraction)imageOut).ResponseHeaders.ContentType = new MimeType ("image/jpg");
					} else {
						((IHttpInteraction)imageOut).ResponseHeaders.ContentType = new MimeType ("image/png");
					}
				}

				MemoryStream imageData = new MemoryStream ();
				SimpleOutgoingInteraction imageSourcer = new SimpleOutgoingInteraction (
					imageData, Encoding.Default, parameters);

				if (Source.TryProcess (imageSourcer)) {
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

