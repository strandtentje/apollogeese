using System.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using BorrehSoft.ApolloGeese.CoreTypes;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Collections.Settings;
using Encoder = System.Drawing.Imaging.Encoder;

namespace Imaging
{
    public class ScaleAndCrop : Service
    {
        public override string Description => "Crop and downscale images";

        protected override void Initialize(Settings settings)
        {
            base.Initialize(settings);
        }

        void IterateSizes(Action<Size, Service> dlg)
        {
            foreach (var item in Branches.Dictionary)
                if (item.Key.StartsWith("to", StringComparison.Ordinal))
                {
                    var sizeDef = item.Key.Substring(2);
                    var sizeParts = sizeDef.Split('x');
                    if (sizeParts.Length == 2)
                    {
                        dlg(
                            new Size(int.Parse(sizeParts[0]), int.Parse(sizeParts[1])),
                            item.Value
                            );
                    }
                }
        }


        protected override bool Process(IInteraction parameters)
        {
            var imageBody = Closest<IIncomingBodiedInteraction>.From(parameters);
            var succ = true;

            using (var image = Image.FromStream(imageBody.IncomingBody))
                IterateSizes((size, srv) =>
                {
                    using (var downscaled = CropAndResize(image, size))                    
                        succ &= srv.TryProcess(new OutgoingImageInteraction(downscaled));
                });

            return succ;
        }

        // gracefully nicked from
        // https://stackoverflow.com/questions/1940581/
        private Image CropAndResize(Image image, Size target)
        {
            var thumbnail = new Bitmap(target.Width, target.Height); // changed parm names
            var graphic = Graphics.FromImage(thumbnail);

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;

            /* ------------------ new code --------------- */

            // Figure out the ratio
            var ratioX = (double)target.Width / (double)image.Width;
            var ratioY = (double)target.Height / (double)image.Height;
            // use whichever multiplier is smaller
            var ratio = ratioX < ratioY ? ratioX : ratioY;

            // now we can get the new height and width
            int newHeight = Convert.ToInt32(image.Height * ratio);
            int newWidth = Convert.ToInt32(image.Width * ratio);

            // Now calculate the X,Y position of the upper-left corner 
            // (one of these will always be zero)
            int posX = Convert.ToInt32((target.Width - (image.Width * ratio)) / 2);
            int posY = Convert.ToInt32((target.Height - (image.Height * ratio)) / 2);

            graphic.Clear(Color.White); // white padding
            graphic.DrawImage(image, posX, posY, newWidth, newHeight);

            return thumbnail;
        }
    }

    internal class OutgoingImageInteraction : SimpleInteraction, IOutgoingBodiedInteraction
    {
        public Encoding Encoding => System.Text.Encoding.ASCII;
        private EncoderParameters EncoderParams { get; }
        private ImageCodecInfo JpgEncoder { get; }
        private Image downscaled;
        private ImageCodecInfo GetEncoder(ImageFormat format) => 
            ImageCodecInfo.GetImageEncoders().First(p => p.FormatID == format.Guid);

        public OutgoingImageInteraction(Image downscaled)
        {
            JpgEncoder = GetEncoder(ImageFormat.Jpeg);
            var encoderQuality = Encoder.Quality;
            EncoderParams = new EncoderParameters(1);
            EncoderParams.Param[0] = new EncoderParameter(encoderQuality, 75L);
            this.downscaled = downscaled;
        }

        public Stream OutgoingBody
        {
            get
            {
                using (var ms = new MemoryStream())
                {
                    downscaled.Save(ms, JpgEncoder, EncoderParams);
                    return ms;
                }
            }
        }
    }
}
