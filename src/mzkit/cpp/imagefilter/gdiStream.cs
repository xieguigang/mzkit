using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace imagefilter
{
    internal class gdiStream
    {

        internal static byte[] getBitmapStream(string fileName)
        {
            return getBitmapStream(Image.FromFile(fileName));
        }

        internal static byte[] getBitmapStream(Image img)
        {
            using (MemoryStream buffer = new MemoryStream())
            {
                img.Save(buffer, ImageFormat.Bmp);
                return buffer.ToArray();
            }
        }
    }
}
