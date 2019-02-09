using System.Drawing;
using System;
using System.IO;
using System.Linq;

namespace TextToImage
{
    class Program
    {
        static void Main()
        {
            var text = @"     TEST STORE

  CUSTOMER RECEIPT

ARTICLE 1      20.00
ARTICLE 2      30.00
--------------------
TOTAL          50.00
VAT 25%        12.50

 DAMN I NEED A LIFE
         :)

Thanks for shopping
   at our store!

S:1  T:2 C:22 R:9999";
            Console.WriteLine("Converting\r\n======================");
            var rows = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            foreach (string row in rows) Console.WriteLine(row);
            Console.WriteLine("======================\r\nto an image and base64 encoded string...");
            using (Bitmap bitmap = ConvertToImage(text))
            {
                var stream = new MemoryStream();
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                var imageBytes = stream.ToArray();
                var base64String = Convert.ToBase64String(imageBytes);
                var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                bitmap.Save(Path.Combine(desktop, "TextToBitmap_Image.png"));
                File.WriteAllText(Path.Combine(desktop, "TextToBitmap_Base64.txt"), base64String);
            }
            Console.WriteLine("\r\nDone. See desktop for output.\r\nPress any key to exit.");
            Console.ReadKey();
        }

        private static Bitmap ConvertToImage(string text)
        {
            var rows = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            var numberOfRows = rows.Length;
            var height = 23 * numberOfRows + 20;
            var width = rows.Max(w => w.Length) * 13;
            var bitmap = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.FillRectangle(Brushes.White, 0, 0, width, height);
                graphics.DrawRectangle(Pens.Black, 0, 0, width - 1, height - 1);
                using (var font = new Font(FontFamily.GenericMonospace, 20, FontStyle.Bold,
                GraphicsUnit.Pixel, 0, false))
                {
                    graphics.DrawString(text, font, Brushes.Black, new RectangleF(0, 10, width, height - 20));
                }
            }
            return bitmap;
        }
    }
}
