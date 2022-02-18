using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace imagefilter
{
    internal class gdiStream
    {

        internal static byte[] getBitmapStream(string fileName)
        {
            Image img = Image.FromFile(fileName);

            using (MemoryStream buffer = new MemoryStream())
            {
                img.Save(buffer, ImageFormat.Bmp);
                return buffer.ToArray();
            }
        }

    }
}
