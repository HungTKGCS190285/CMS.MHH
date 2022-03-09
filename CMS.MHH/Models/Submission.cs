using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CMS.MHH.Models
{
    public class Submission
    {
        public int Id { get; set; }

        [Display(Name = "Submission name")]
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Closure_date { get; set; }

        public DateTime Final_closure_date { get; set; }
    }
}