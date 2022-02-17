using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CMS.MHH.Models
{
    public class Comment
    {
        public Comment()
        {
            this.Date = DateTime.Now;
        }
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }
        [Required]
        public DateTime Date { get; set; }

        public string AuthorId { get; set; }

        public string AuthorName { get; set; }

        public bool IsAnonymous { get; set; }

        public virtual ApplicationUser Author { get; set; }

        public int IdeasId { get; set; }
        public virtual Idea Ideas { get; set; }

    }
}