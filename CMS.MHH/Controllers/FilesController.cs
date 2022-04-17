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
            //fetch all the available files in the path
            string[] filsewithPath = Directory.GetFiles(Server.MapPath("~/Files"));
            
            //prepare a List which contain all the files
            List<FileVM> files = new List<FileVM>();

            //use the loop to add file to the list
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

        public ActionResult Download_DocbySubmission()
            {
            var submit = db.Submissions.ToList();
            List<Export_IdeaVM> export_idea = new List<Export_IdeaVM>();
            foreach (var idea in submit)
            {
                export_idea.Add(new Export_IdeaVM()
                {
                    SubmissionId = idea.Id,
                    SubmissionName = idea.Name,
                    Content = "The Document of submission " + idea.Name + " is available to download!"
                });
            }
            return View(export_idea.ToList());
        }

        public ActionResult Download_Doc(int id)
        {
            var ideas = db.Ideas.Where(x => x.SubmissionId == id);
            using (ZipFile filezip = new ZipFile())
            {
                filezip.AlternateEncodingUsage = ZipOption.AsNecessary;
                filezip.AddDirectoryByName(Server.MapPath("Files"));
                foreach (var idea in ideas)
                {
                    if (idea.DocumentName != null)
                    {
                        var file_paths = Directory.GetFiles(Server.MapPath("/Files"))
                                                  .Select(f => Path.GetFileName(f))
                                                  .Where(f => f.StartsWith(idea.DocumentName)).FirstOrDefault();

                        var path = Server.MapPath("~/Files/"+file_paths);
                        filezip.AddFile(path, "Files");
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
            //Add the header for the file content
            StringWriter sw = new StringWriter();
            sw.WriteLine("\"Id\",\"Title\",\"IsAnonymous\",\"SubmissionId\",\"Author\",\"Category Name\",\"Document Name\",\"Description\",\"Content\",\"Date\",\"View\",\"Thums Up\",\"Thumbs Down\"");
            
            //config and prepare the file
            Response.ClearContent();
            Response.AddHeader("content-disposition",
                                string.Format("attachment;filename=Export_Data_{0}.csv", DateTime.Now));
            Response.ContentType = "text/csv";

            var list = db.Ideas.Where(x => x.SubmissionId == id).ToList();

            //var list_ideas = db.Ideas.OrderBy(x => x.Id).ToList();
            foreach (var idea in list)
            {
                //write the content which match the header
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

            //Create the sw completely content file
            Response.Write(sw.ToString());
            Response.End();
            return RedirectToAction("Export_Idea");
        }


    }
}