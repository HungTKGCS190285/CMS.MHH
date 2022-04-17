using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.MHH.Models
{
    public class Reaction
    {
        public int Id { get; set; }

        public bool IsLike { get; set; }

        public string AuthorId { get; set; }

        public virtual ApplicationUser Author { get; set; }

        public int PostId { get; set; }

        public virtual Idea Post { get; set; }
    }
}