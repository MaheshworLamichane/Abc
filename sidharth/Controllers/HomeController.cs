using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using sidharth.Models;
using System.Diagnostics;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using ICSharpCode.SharpZipLib.Zip;

namespace sidharth.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public FileResult GenerateAndDownloadZip()
        {

            var webRoot = _hostingEnvironment.WebRootPath;
            var fileName = "MyZip.zip";
            var tempOutput = webRoot + "/Image/" + fileName;

            using (ZipOutputStream oZipoutputStream = new ZipOutputStream(System.IO.File.Create(tempOutput)))
            {
                oZipoutputStream.SetLevel(9);
                byte[] buffer = new byte[4096];
                var ImageList = new List<string>();
                ImageList.Add(webRoot + "/Image/Screenshot1.png");
                ImageList.Add(webRoot + "/Image/Screenshot2.png");

                for (int i = 0; i < ImageList.Count; i++)
                {
                    ZipEntry entry = new ZipEntry(Path.GetFileName(ImageList[i]));
                    entry.DateTime = DateTime.Now;
                    entry.IsUnicodeText = true;
                    oZipoutputStream.PutNextEntry(entry);

                    using (FileStream oFileStream = System.IO.File.OpenRead(ImageList[i]))
                    {
                        int sourceBytes;
                        do
                        {
                            sourceBytes = oFileStream.Read(buffer, 0, buffer.Length);
                            oZipoutputStream.Write(buffer, 0, sourceBytes);

                        } while (sourceBytes > 0);

                    }

                }
                oZipoutputStream.Finish();
                oZipoutputStream.Flush();
                oZipoutputStream.Close();


            }
            byte[] finalResult = System.IO.File.ReadAllBytes(tempOutput);
            if (System.IO.File.Exists(tempOutput))
            {
                System.IO.File.Delete(tempOutput);
            }
            if (finalResult == null || !finalResult.Any())
            {
                throw new Exception(String.Format("Nothing Found"));
            }
            return File(finalResult, "application/zip", fileName);
        }
    }
}


