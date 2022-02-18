using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CMS.MHH.ViewModel
{
    public class User_Role
    {
        public string UserId { get; set; }

        [Display(Name = "Name")]
        public string Username { get; set; }

        [Display(Name = "Email")]
        public string UserEmail { get; set; }

        [Display(Name = "Role")]
        public string UserRole { get; set; }

        [Display(Name = "Phone Number")]
        public string Phonenumber { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }
    }
}