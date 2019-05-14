using System;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.IO;
using System.DrawingCore.Imaging;
using System.DrawingCore;

namespace SinaBlogAlbumExport
{
    class Program
    {
        static void Main(string[] args)
        {

            var xmlPath = "blog.xml";

            var savePath = "Images";

            if (Directory.Exists(savePath))
            {
                DeleteDirectory(savePath);
            }
            Directory.CreateDirectory(savePath);

            XDocument myXDoc = XDocument.Load(xmlPath);
            var RootElemnt = myXDoc.Element("PhotoList");
            var photos = RootElemnt.Elements().Select(b => b.Elements()).SelectMany(b => b.Select(v => v.Value));

            Console.WriteLine($"共{photos.Count()}张照片");

            var index = 1;
            photos.ToList().ForEach(b =>
            {
                Console.WriteLine($"正在导出第{index}张");
                SaveImage(b, Path.Combine(savePath, $"image_${index}.jpg"), ImageFormat.Jpeg);
                index++;
            });

            Console.WriteLine("finished");
        }


        public static void SaveImage(string imageUrl, string filename, ImageFormat format)
        {

            WebClient client = new WebClient();
            Stream stream = client.OpenRead(imageUrl);
            Bitmap bitmap; bitmap = new Bitmap(stream);

            if (bitmap != null)
                bitmap.Save(filename, format);

            stream.Flush();
            stream.Close();
            client.Dispose();
        }


        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }
    }
}
