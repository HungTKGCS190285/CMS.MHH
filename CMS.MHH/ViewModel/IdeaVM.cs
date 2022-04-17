using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.MHH.ViewModel
{
    public class IdeaVM
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string CategoryName { get; set; }

        public DateTime Date { get; set; }

        public DateTime LastModify { get; set; }

        public string AuthorId { get; set; }

        public string Author { get; set; }

        public int View { get; set; }
        
        public string DocumentName { get; set; }
        public IEnumerable<CommentVM> comments { get; set; }
    }
}