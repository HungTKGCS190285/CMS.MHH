using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.MHH.ViewModel
{
    public class StatisticsVM
    {
        public decimal num_idea_QA { get; set; }

        public decimal num_idea_IT { get; set; }

        public decimal num_idea_HR { get; set; }

        public decimal per_idea_QA { get; set; }

        public decimal per_idea_IT { get; set; }

        public decimal per_idea_HR { get; set; }

        public int num_contri_QA { get; set; }

        public int num_contri_IT { get; set; }

        public int num_contri_HR { get; set; }

        public int idea_no_comment { get; set; }

        public int idea_anonymous { get; set; }

        public int comment_anonymous { get; set; }
    }
}