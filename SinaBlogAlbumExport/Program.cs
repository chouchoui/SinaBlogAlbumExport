using System;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace SinaBlogAlbumExport
{
    class Program
    {
        static void Main(string[] args)
        {
            var xmlPath = "blog.xml";

            var savePath = "Images";

            Console.Write("请输入开始导出的照片序号（默认为1）：");
            if (!int.TryParse(Console.ReadLine(), out int index))
            {
                index = 1;
            }


            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }


            XDocument myXDoc = XDocument.Load(xmlPath);
            var RootElemnt = myXDoc.Element("PhotoList");
            var photos = RootElemnt.Elements().Select(b => b.Elements()).SelectMany(b => b.Select(v => v.Value));

            Console.WriteLine($"共{photos.Count()}张照片");

            photos.Skip(index - 1).ToList().ForEach(b =>
            {
                var imagePath = Path.Combine(savePath, $"image_{index}.jpg");

                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }

                Console.WriteLine($"正在导出第{index}张");
                SaveImage(b, imagePath, ImageFormat.Jpeg);
                index++;
            });
            Console.WriteLine("************************************************************");
            Console.WriteLine("导出完成，按任意键关闭");
            Console.ReadKey();
        }


        public static void SaveImage(string imageUrl, string filename, ImageFormat format)
        {

            WebClient client = new WebClient();
            Stream stream = client.OpenRead(imageUrl);
            Bitmap bitmap = new Bitmap(stream);

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
