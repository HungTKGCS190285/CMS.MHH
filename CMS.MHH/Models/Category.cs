using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CMS.MHH.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Display(Name="Category")]
        public string Category_Name { get; set; }
    }
}