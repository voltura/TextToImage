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

\x{WIDE} DAMN I NEED A LIFE
         :)

Thanks for shopping
   at our store!

S:1  T:2 C:22 R:9999";
            Console.WriteLine("Converting\r\n======================");
            var rows = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            var receiptWithoutFormatting = string.Empty;
            var builder = new System.Text.StringBuilder();
            builder.Append(receiptWithoutFormatting);
            foreach (string row in rows)
            {
                Console.WriteLine(row);
                builder.Append(RemoveEscapeSequences(row) + "\r\n");
            }
            receiptWithoutFormatting = builder.ToString().TrimEnd(new char[] { '\r', '\n' });
            Console.WriteLine("======================\r\nto an image and base64 encoded string\r\nafter stripping escape sequences...");
            using (Bitmap bitmap = ConvertToImage(receiptWithoutFormatting))
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

        private static string RemoveEscapeSequences(string text)
        {
            var escapeStart = text.IndexOf(@"\x");
            if (escapeStart == -1)
                return text;
            var escapeEnd = text.LastIndexOf('}') + 1;
            var escapeString = text.Substring(escapeStart, escapeEnd);
            return text.Replace(escapeString, "");
        }
    }
}
