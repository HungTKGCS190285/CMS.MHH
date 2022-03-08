using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CMS.MHH.Models
{
    public class Idea
    {

        public Idea()
        {
            this.Comments = new HashSet<Comment>();
        }

        public int Id { get; set; }

        public int CateId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string CateName { get; set; }

        public virtual Category Cate { get; set; }

        public string AuthorId { get; set; }

        //
        public string Author_Email { get; set; }

        public virtual ApplicationUser Author { get; set; }

        public int SubmissionId { get; set; }

        public virtual Submission Submission { get; set; }

        [Display(Name = "Is Anonymous")]
        public bool IsAnonymous { get; set; }

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }

        public DateTime Date { get; set; }
        public DateTime LastModify { get; set; }

        [Display(Name = "Uploaded file")]
        public string DocumentName { get; set; }

        public int View { get; set; }

        public int ThumbsUp { get; set; }

        public int ThumbsDown { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

    }
}