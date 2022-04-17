using CMS.MHH.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public IDbSet<Idea> Ideas { get; set; }

    public IDbSet<Comment> Comments { get; set; }

    public IDbSet<Category> Categories { get; set; }

    public IDbSet<Reaction> Reactions { get; set; }

    public IDbSet<Department> Departments { get; set; }

    public IDbSet<Submission> Submissions { get; set; }

    //public IEnumerable<IdeaViewModel> IdeaViewModels { get; set; }

    public ApplicationDbContext()
        : base("DefaultConnection", throwIfV1Schema: false)
    {
    }

    public static ApplicationDbContext Create()
    {
        return new ApplicationDbContext();
    }
}
