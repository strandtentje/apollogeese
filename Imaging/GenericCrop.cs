using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Settings;
using System.Drawing;
using System.Drawing.Imaging;

namespace Imaging
{
	public abstract class GenericCrop : ImagingService
	{
		public override string Description {
			get {
				return "crop";
			}
		}

		protected abstract Rectangle GetBounds (IInteraction parameters);

		Rectangle GetClippedBounds (Size size, IInteraction parameters)
		{
			Rectangle crop = GetBounds (parameters);
			Rectangle full = new Rectangle (Point.Empty, size);

			return Rectangle.Intersect (full, crop);
		}

		protected override bool Process (IInteraction parameters)
		{
			return TryGetImage (parameters, delegate(Image inImage) {
				Rectangle clippedBounds = GetClippedBounds(inImage.Size, parameters);

				if (clippedBounds.Equals(Rectangle.Empty)) {
					return emptyBitmap;
				} else {
					Bitmap target = new Bitmap(clippedBounds.Width, clippedBounds.Height, PixelFormat.Format32bppArgb);
					Graphics g = Graphics.FromImage(target);
					g.DrawImage(inImage, new RectangleF(new Point(0, 0), target.Size), clippedBounds, GraphicsUnit.Pixel);
					g.Flush();

					return target;
				}
			});
		}
	}
}

