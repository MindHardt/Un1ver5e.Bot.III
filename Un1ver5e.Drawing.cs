using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Un1ver5e.Bot
{
    public static class Drawing
    {
        private static readonly string _galleryPath = Statics.DataFolderPath + "/Gallery";
        public static Stream CreateLetter(string message)
        {
            //Loading image
            Image image = Image.Load(_galleryPath + "/Image.Paper.jpg");


            //Creating a font
            FontCollection collection = new();
            FontFamily family = SystemFonts.Get("ScriptC");
            Font font = family.CreateFont(64, FontStyle.BoldItalic);

            //Text options
            TextOptions options = new(font)
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Font = font,
                WordBreaking = WordBreaking.BreakAll,
                Dpi = 72,
                LineSpacing = 0.5f,
                WrappingLength = 1300
            };
            //Working space rectangle (50px offset from each size)
            Rectangle rect = new(50, 50, 1340, 1880);

            //Drawing text
            image.Mutate(
                x => x.Crop(rect).DrawText(options, message, Color.Black));

            //Saving image to stream
            MemoryStream ms = new();
            image.Save(ms, new JpegEncoder());

            return ms;
        }
    }
}
