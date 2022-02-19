using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.MHH.ViewModel
{
    public class CommentVM
    {
        public string Comment { get; set; }
        public string CommentAuthor { get; set; }
        public DateTime CommentDate { get; set; }

        public bool CommentAnony { get; set; }
    }
}