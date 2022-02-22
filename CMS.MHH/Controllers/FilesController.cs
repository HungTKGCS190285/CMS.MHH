using CMS.MHH.ViewModel;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.MHH.Controllers
{
    public class FilesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Files
        public ActionResult Indexs()
        {
            string[] filsewithPath = Directory.GetFiles(Server.MapPath("~/Files"));
            List<FileVM> files = new List<FileVM>();
            foreach (string filewithPath in filsewithPath)
            {
                files.Add(new FileVM()
                {
                    FileName = Path.GetFileName(filewithPath),
                    FilePath = filewithPath
                });
            }
            return View(files.ToList());
        }
        
        [HttpPost]
        public ActionResult CreateZipFile(List<FileVM> files)
        {
            using (ZipFile filezip = new ZipFile())
            {
                filezip.AlternateEncodingUsage = ZipOption.AsNecessary;
                filezip.AddDirectoryByName("Files");
                foreach (FileVM file in files)
                {
                    if (file.IsChosen)
                    {
                        filezip.AddFile(file.FilePath, "Files");
                    }
                }
                string namezip = String.Format("Zip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss0"));
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    filezip.Save(memoryStream);
                    return File(memoryStream.ToArray(), "application/zip", namezip);
                }
            }
        }

        public ActionResult Export_Idea()
        {
            var submit = db.Submissions.ToList();
            List<Export_IdeaVM> export_idea = new List<Export_IdeaVM>();
            foreach (var idea in submit)
            {
                export_idea.Add(new Export_IdeaVM()
                {
                    SubmissionId = idea.Id,
                    SubmissionName = idea.Name,
                    Content = "The submission " + idea.Name + " is available to download!"
                });
            }
            return View(export_idea.ToList());
        }

        public ActionResult Export(int id)
        {
            StringWriter sw = new StringWriter();
            sw.WriteLine("\"Id\",\"Title\",\"IsAnonymous\",\"SubmissionId\",\"Author\",\"Category Name\",\"Document Name\",\"Description\",\"Content\",\"Date\",\"View\",\"Thums Up\",\"Thumbs Down\"");
            Response.ClearContent();
            Response.AddHeader("content-disposition",
                                string.Format("attachment;filename=Export_Data_{0}.csv", DateTime.Now));
            Response.ContentType = "text/csv";

            var list = db.Ideas.Where(x => x.SubmissionId == id).ToList();

            //var list_ideas = db.Ideas.OrderBy(x => x.Id).ToList();
            foreach (var idea in list)
            {
                sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\"",
                                           idea.Id,
                                           idea.Title,
                                           idea.IsAnonymous,
                                           idea.SubmissionId,
                                           idea.AuthorId,
                                           idea.CateId,
                                           idea.DocumentName,
                                           idea.Description,
                                           idea.Content.ToString(),
                                           idea.Date,
                                           idea.View,
                                           idea.ThumbsUp,
                                           idea.ThumbsDown
                    ));
            }
            Response.Write(sw.ToString());
            Response.End();
            return View("Export_Idea");
        }


    }
}