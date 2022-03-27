using CMS.MHH.Models;
using CMS.MHH.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.MHH.Controllers
{
    public class StatisticsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            var total_ideas = db.Ideas.Count();

            decimal num_idea_QA = 0; //number idea in QA depart
            decimal num_idea_IT = 0; //number idea in IT depart
            decimal num_idea_HR = 0; //number idea in HR depart

            int con_IT = 0; //contributor of IT
            int con_QA = 0; //contributor of QA
            int con_HR = 0; //contributor of HR

            
            // Count number of idea in department
            var ideas = db.Ideas.ToList();
            foreach (var idea in ideas)
            {
                var user = db.Users.Where(x => x.Id == idea.AuthorId).FirstOrDefault();
                if (user.Department.Name == "QA")
                {
                    num_idea_QA++;
                }
                else if (user.Department.Name == "IT")
                {
                    num_idea_IT++;
                }
                else if (user.Department.Name == "HR")
                {
                    num_idea_HR++;
                }
            }


            //Count number of contributors in all Department
            
            List<ApplicationUser> users = new List<ApplicationUser>(); // Create new empty list which contain User 

            int i = 0;
            foreach (var idea in ideas) //fetch and read each idea in the list of ideas
            {                
                var user_idea = db.Users.Where(x => x.Id == idea.AuthorId).FirstOrDefault();
                
                if (i == 0) //Allow the system add the first user from first idea to the "users" list
                {
                    users.Add(user_idea);
                    i++;
                }
                else
                {
                    // from the second ideae, execute check the author of idea is exist in the "users"
                    
                    int j = 0; //count the time of author exist 
                    foreach (var user in users)
                    {
                        if (user.Id  == user_idea.Id)
                        {
                            j++;
                        }
                    }
                    if (j == 0) // if they dont exist, add them to the "users"
                    {
                        users.Add(user_idea);
                    }
                }

            }

            //Count contributor in each department
            foreach (var user in users)
            {
                if (user.Department.Name == "QA")
                {
                    con_QA++;
                }
                else if (user.Department.Name == "IT")
                {
                    con_IT++;
                }
                else if (user.Department.Name == "HR")
                {
                    con_HR++;
                }
            }

            var statistic = new StatisticsVM();
            {
                statistic.num_idea_IT = num_idea_IT;
                statistic.num_idea_QA = num_idea_QA;
                statistic.num_idea_HR = num_idea_HR;


                statistic.per_idea_IT = Math.Round((100 * (num_idea_IT / total_ideas)), 2);
                statistic.per_idea_QA = Math.Round((100 * (num_idea_QA / total_ideas)), 2);
                statistic.per_idea_HR = Math.Round((100 * (num_idea_HR / total_ideas)), 2);

                statistic.num_contri_IT = con_IT;
                statistic.num_contri_QA = con_QA;
                statistic.num_contri_HR = con_HR;

            }

            return View(statistic);
        }

        public ActionResult Exceptions() //Count number of anonymous ideas, comments, and no comments
        {

            var ideas = db.Ideas.ToList();
            List<Idea> ideas1 = new List<Idea>();
            int idea_anony = 0;
            int cmt_anony = 0;
            foreach (var idea in ideas)
            {
                if (idea.IsAnonymous == true)
                {
                    idea_anony++;
                }
                var coms = db.Comments.Where(x => x.IdeasId == idea.Id).ToList();
                if (coms.Count() == 0)
                {
                    ideas1.Add(idea);
                }
            }

            var comments = db.Comments.ToList();
            foreach (var comment in comments)
            {
                if (comment.IsAnonymous == true)
                {
                    cmt_anony++;
                }
            }
            var statistics = new StatisticsVM();
            {
                statistics.idea_no_comment = ideas1.Count();
                statistics.idea_anonymous = idea_anony;
                statistics.comment_anonymous = cmt_anony;
            }
            return View(statistics);
        }

        public ActionResult Pie_Chart_idea()
        {
            var total_ideas = db.Ideas.Count();

            decimal num_idea_QA = 0;
            decimal num_idea_IT = 0;
            decimal num_idea_HR = 0;

            int con_IT = 0;
            int con_QA = 0;
            int con_HR = 0;


            var ideas = db.Ideas.ToList();
            foreach (var idea in ideas)
            {
                var user = db.Users.Where(x => x.Id == idea.AuthorId).FirstOrDefault();
                if (user.Department.Name == "QA")
                {
                    num_idea_QA++;
                }
                else if (user.Department.Name == "IT")
                {
                    num_idea_IT++;
                }
                else if (user.Department.Name == "HR")
                {
                    num_idea_HR++;
                }
            }
            List<ApplicationUser> users = new List<ApplicationUser>();

            int i = 0;
            foreach (var idea in ideas)
            {
                var user_idea = db.Users.Where(x => x.Id == idea.AuthorId).FirstOrDefault();
                if (i == 0)
                {
                    users.Add(user_idea);
                    i++;
                }
                else
                {
                    int j = 0;
                    foreach (var user in users)
                    {
                        if (user.Id == user_idea.Id)
                        {
                            j++;
                        }
                    }
                    if (j == 0)
                    {
                        users.Add(user_idea);
                    }
                }

            }

            foreach (var user in users)
            {
                if (user.Department.Name == "QA")
                {
                    con_QA++;
                }
                else if (user.Department.Name == "IT")
                {
                    con_IT++;
                }
                else if (user.Department.Name == "HR")
                {
                    con_HR++;
                }
            }

            var statistic = new StatisticsVM();
            {
                statistic.num_idea_IT = num_idea_IT;
                statistic.num_idea_QA = num_idea_QA;
                statistic.num_idea_HR = num_idea_HR;


                statistic.per_idea_IT = Math.Round((100 * (num_idea_IT / total_ideas)), 2);
                statistic.per_idea_QA = Math.Round((100 * (num_idea_QA / total_ideas)), 2);
                statistic.per_idea_HR = Math.Round((100 * (num_idea_HR / total_ideas)), 2);

                statistic.num_contri_IT = con_IT;
                statistic.num_contri_QA = con_QA;
                statistic.num_contri_HR = con_HR;

            }

            return View(statistic);
        }

        public ActionResult Number_idea_chart()
        {
            var total_ideas = db.Ideas.Count();

            decimal num_idea_QA = 0;
            decimal num_idea_IT = 0;
            decimal num_idea_HR = 0;

            int con_IT = 0;
            int con_QA = 0;
            int con_HR = 0;


            var ideas = db.Ideas.ToList();
            foreach (var idea in ideas)
            {
                var user = db.Users.Where(x => x.Id == idea.AuthorId).FirstOrDefault();
                if (user.Department.Name == "QA")
                {
                    num_idea_QA++;
                }
                else if (user.Department.Name == "IT")
                {
                    num_idea_IT++;
                }
                else if (user.Department.Name == "HR")
                {
                    num_idea_HR++;
                }
            }
            List<ApplicationUser> users = new List<ApplicationUser>();

            int i = 0;
            foreach (var idea in ideas)
            {
                var user_idea = db.Users.Where(x => x.Id == idea.AuthorId).FirstOrDefault();
                if (i == 0)
                {
                    users.Add(user_idea);
                    i++;
                }
                else
                {
                    int j = 0;
                    foreach (var user in users)
                    {
                        if (user.Id == user_idea.Id)
                        {
                            j++;
                        }
                    }
                    if (j == 0)
                    {
                        users.Add(user_idea);
                    }
                }

            }

            foreach (var user in users)
            {
                if (user.Department.Name == "QA")
                {
                    con_QA++;
                }
                else if (user.Department.Name == "IT")
                {
                    con_IT++;
                }
                else if (user.Department.Name == "HR")
                {
                    con_HR++;
                }
            }

            var statistic = new StatisticsVM();
            {
                statistic.num_idea_IT = num_idea_IT;
                statistic.num_idea_QA = num_idea_QA;
                statistic.num_idea_HR = num_idea_HR;


                statistic.per_idea_IT = Math.Round((100 * (num_idea_IT / total_ideas)), 2);
                statistic.per_idea_QA = Math.Round((100 * (num_idea_QA / total_ideas)), 2);
                statistic.per_idea_HR = Math.Round((100 * (num_idea_HR / total_ideas)), 2);

                statistic.num_contri_IT = con_IT;
                statistic.num_contri_QA = con_QA;
                statistic.num_contri_HR = con_HR;

            }

            return View(statistic);
        }

        public ActionResult Number_contributor_chart()
        {
            var total_ideas = db.Ideas.Count();

            decimal num_idea_QA = 0;
            decimal num_idea_IT = 0;
            decimal num_idea_HR = 0;

            int con_IT = 0;
            int con_QA = 0;
            int con_HR = 0;


            var ideas = db.Ideas.ToList();
            foreach (var idea in ideas)
            {
                var user = db.Users.Where(x => x.Id == idea.AuthorId).FirstOrDefault();
                if (user.Department.Name == "QA")
                {
                    num_idea_QA++;
                }
                else if (user.Department.Name == "IT")
                {
                    num_idea_IT++;
                }
                else if (user.Department.Name == "HR")
                {
                    num_idea_HR++;
                }
            }
            List<ApplicationUser> users = new List<ApplicationUser>();

            int i = 0;
            foreach (var idea in ideas)
            {
                var user_idea = db.Users.Where(x => x.Id == idea.AuthorId).FirstOrDefault();
                if (i == 0)
                {
                    users.Add(user_idea);
                    i++;
                }
                else
                {
                    int j = 0;
                    foreach (var user in users)
                    {
                        if (user.Id == user_idea.Id)
                        {
                            j++;
                        }
                    }
                    if (j == 0)
                    {
                        users.Add(user_idea);
                    }
                }

            }

            foreach (var user in users)
            {
                if (user.Department.Name == "QA")
                {
                    con_QA++;
                }
                else if (user.Department.Name == "IT")
                {
                    con_IT++;
                }
                else if (user.Department.Name == "HR")
                {
                    con_HR++;
                }
            }

            var statistic = new StatisticsVM();
            {
                statistic.num_idea_IT = num_idea_IT;
                statistic.num_idea_QA = num_idea_QA;
                statistic.num_idea_HR = num_idea_HR;


                statistic.per_idea_IT = Math.Round((100 * (num_idea_IT / total_ideas)), 2);
                statistic.per_idea_QA = Math.Round((100 * (num_idea_QA / total_ideas)), 2);
                statistic.per_idea_HR = Math.Round((100 * (num_idea_HR / total_ideas)), 2);

                statistic.num_contri_IT = con_IT;
                statistic.num_contri_QA = con_QA;
                statistic.num_contri_HR = con_HR;

            }

            return View(statistic);
        }
    }

}