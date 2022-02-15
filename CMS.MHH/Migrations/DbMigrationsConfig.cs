namespace CMS.MHH.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using CMS.MHH.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    internal sealed class DbMigrationsConfig : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public DbMigrationsConfig()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            if (!context.Users.Any())
            {
                // ADMIN user
                var adminEmail = "admin@fpt.com";
                var adminUserName = adminEmail;
                var adminPassword = "1234";
                string adminRole = "Admin";
                string adminDepartment = "Administrator";
                CreateAdmin(context, adminEmail, adminUserName, adminPassword, adminRole, adminDepartment);

                //QA Manager user
                var QA_M_Email = "QA_Manager@fpt.com";
                var QA_M_UserName = QA_M_Email;
                var QA_M_Password = "1234";
                string[] QA_M_Role = { "Staff", "QA Manager" };
                string QA_M_Department = "QA";
                CreateQA_Manager(context, QA_M_Email, QA_M_UserName, QA_M_Password, QA_M_Role, QA_M_Department);

                //QA Coordinator IT user
                var QA_IT_Email = "QA_IT@fpt.com";
                var QA_IT_UserName = QA_IT_Email;
                var QA_IT_Password = "1234";
                string[] QA_IT_Role = { "Staff", "QA_C" };
                string QA_IT_Department = "IT";
                CreateQA_IT(context, QA_IT_Email, QA_IT_UserName, QA_IT_Password, QA_IT_Role, QA_IT_Department);

                //QA Coordinator HR user
                var QA_HR_Email = "QA_HR@fpt.com";
                var QA_HR_UserName = QA_HR_Email;
                var QA_HR_Password = "1234";
                string[] QA_HR_Role = { "Staff", "QA_C" };
                string QA_HR_Department = "HR";
                CreateQA_HR(context, QA_HR_Email, QA_HR_UserName, QA_HR_Password, QA_HR_Role, QA_HR_Department);

                //Staff in QA department user
                var Staff_QA_Email = "Staff_QA@fpt.com";
                var Staff_QA_UserName = Staff_QA_Email;
                var Staff_QA_Password = "1234";
                string[] Staff_QA_Role = { "Staff" };
                string Staff_QA_Department = "QA";
                Create_Staff_QA(context, Staff_QA_Email, Staff_QA_UserName, Staff_QA_Password, Staff_QA_Role, Staff_QA_Department);

                //staff in HR department user
                var Staff_HR_Email = "Staff_HR@fpt.com";
                var Staff_HR_UserName = Staff_HR_Email;
                var Staff_HR_Password = "1234";
                string[] Staff_HR_Role = { "Staff" };
                string Staff_HR_Department = "HR";
                Create_Staff_HR(context, Staff_HR_Email, Staff_HR_UserName, Staff_HR_Password, Staff_HR_Role, Staff_HR_Department);

                //staff in IT department user
                var Staff_IT_Email = "Staff_IT@fpt.com";
                var Staff_IT_UserName = Staff_IT_Email;
                var Staff_IT_Password = "1234";
                string[] Staff_IT_Role = { "Staff" };
                string Staff_IT_Department = "IT";
                Create_Staff_IT(context, Staff_IT_Email, Staff_IT_UserName, Staff_IT_Password, Staff_IT_Role, Staff_IT_Department);

                CreateSeveralCategory(context);
                CreateSeveralEvents(context);


            }
        }

        private void CreateSeveralCategory(ApplicationDbContext context)
        {
            context.Categories.Add(new Category()
            {
                Category_Name = "Sport",
            });

            context.Categories.Add(new Category()
            {
                Category_Name = "Holiday",
            });
            context.Categories.Add(new Category()
            {
                Category_Name = "Public Event",
            });

        }

        private void CreateSeveralEvents(ApplicationDbContext context)
        {
            context.Ideas.Add(new Idea()
            {
                Content = "1st Idea",
                CateId = 1,
                Date = DateTime.Now.AddDays(5).AddHours(21).AddMinutes(30),
                Author = context.Users.First(),
                Author_Email = context.Users.First().Email
            });
            context.Ideas.Add(new Idea()
            {
                Content = "2nd Idea",
                CateId = 2,
                Date = DateTime.Now.AddDays(2).AddHours(10).AddMinutes(30),
                IsAnonymous = true,
                Author = context.Users.First(),
                Author_Email = context.Users.First().Email,
                Comments = new HashSet<Comment>()
                {
                    new Comment() {Text = "<Anonymous> comment", Author = context.Users.First(), IsAnonymous = true, AuthorName = context.Users.First().Email   },
                    new Comment() {Text = "UserComment", Author = context.Users.First(), IsAnonymous = false, AuthorName = context.Users.First().Email}
                }
            });
        }


        private void CreateAdmin(ApplicationDbContext context, string adminEmail, string adminUserName, string adminPassword, string adminRole, string adminDepartment)
        {

            var adminUser = new ApplicationUser
            {
                UserName = adminUserName,
                Email = adminEmail,
                Department = adminDepartment
            };
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 4,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            var userCreateResult = userManager.Create(adminUser, adminPassword);
            if (!userCreateResult.Succeeded)
            {
                throw new Exception(string.Join("; ", userCreateResult.Errors));
            }


            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var roleCreateResult = roleManager.Create(new IdentityRole(adminRole));
            if (!roleCreateResult.Succeeded)
            {
                throw new Exception(string.Join("; ", roleCreateResult.Errors));
            }

            var addTrainingstaffRoleResult = userManager.AddToRole(adminUser.Id, adminRole);
            if (!addTrainingstaffRoleResult.Succeeded)
            {
                throw new Exception(string.Join("; ", addTrainingstaffRoleResult.Errors));
            }

        }

        private void CreateQA_Manager(ApplicationDbContext context, string QA_M_Email, string QA_M_UserName, string QA_M_Password, string[] QA_M_Role, string QA_M_Department)
        {
            var adminUser = new ApplicationUser
            {
                UserName = QA_M_UserName,
                Email = QA_M_Email,
                Department = QA_M_Department
            };
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 4,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            var userCreateResult = userManager.Create(adminUser, QA_M_Password);
            if (!userCreateResult.Succeeded)
            {
                throw new Exception(string.Join("; ", userCreateResult.Errors));
            }

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            foreach (var roleName in QA_M_Role)
            {
                var roleExist = roleManager.RoleExists(roleName);
                if (!roleExist)
                {
                    var role = new IdentityRole();
                    role.Name = roleName;
                    var roleCreateResult = roleManager.Create(new IdentityRole(roleName));
                    if (!roleCreateResult.Succeeded)
                    {
                        throw new Exception(string.Join("; ", roleCreateResult.Errors));
                    }
                    var addTrainingstaffRoleResult = userManager.AddToRole(adminUser.Id, roleName);
                    if (!addTrainingstaffRoleResult.Succeeded)
                    {
                        throw new Exception(string.Join("; ", addTrainingstaffRoleResult.Errors));
                    }
                }
            }


        }

        private void CreateQA_IT(ApplicationDbContext context, string QA_M_Email, string QA_M_UserName, string QA_M_Password, string[] QA_M_Role, string QA_M_Department)
        {
            var adminUser = new ApplicationUser
            {
                UserName = QA_M_UserName,
                Email = QA_M_Email,
                Department = QA_M_Department
            };
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 4,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            var userCreateResult = userManager.Create(adminUser, QA_M_Password);
            if (!userCreateResult.Succeeded)
            {
                throw new Exception(string.Join("; ", userCreateResult.Errors));
            }

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            foreach (var roleName in QA_M_Role)
            {
                var roleExist = roleManager.RoleExists(roleName);
                if (!roleExist)
                {
                    var role = new IdentityRole();
                    role.Name = roleName;
                    var roleCreateResult = roleManager.Create(new IdentityRole(roleName));
                    if (!roleCreateResult.Succeeded)
                    {
                        throw new Exception(string.Join("; ", roleCreateResult.Errors));
                    }
                    var addTrainingstaffRoleResult = userManager.AddToRole(adminUser.Id, roleName);
                    if (!addTrainingstaffRoleResult.Succeeded)
                    {
                        throw new Exception(string.Join("; ", addTrainingstaffRoleResult.Errors));
                    }
                }
            }


        }

        private void CreateQA_HR(ApplicationDbContext context, string QA_M_Email, string QA_M_UserName, string QA_M_Password, string[] QA_M_Role, string QA_M_Department)
        {
            var adminUser = new ApplicationUser
            {
                UserName = QA_M_UserName,
                Email = QA_M_Email,
                Department = QA_M_Department
            };
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 4,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            var userCreateResult = userManager.Create(adminUser, QA_M_Password);
            if (!userCreateResult.Succeeded)
            {
                throw new Exception(string.Join("; ", userCreateResult.Errors));
            }

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            foreach (var roleName in QA_M_Role)
            {
                var roleExist = roleManager.RoleExists(roleName);
                if (!roleExist)
                {
                    var role = new IdentityRole();
                    role.Name = roleName;
                    var roleCreateResult = roleManager.Create(new IdentityRole(roleName));
                    if (!roleCreateResult.Succeeded)
                    {
                        throw new Exception(string.Join("; ", roleCreateResult.Errors));
                    }
                    var addTrainingstaffRoleResult = userManager.AddToRole(adminUser.Id, roleName);
                    if (!addTrainingstaffRoleResult.Succeeded)
                    {
                        throw new Exception(string.Join("; ", addTrainingstaffRoleResult.Errors));
                    }
                }
            }


        }

        private void Create_Staff_QA(ApplicationDbContext context, string QA_M_Email, string QA_M_UserName, string QA_M_Password, string[] QA_M_Role, string QA_M_Department)
        {
            var adminUser = new ApplicationUser
            {
                UserName = QA_M_UserName,
                Email = QA_M_Email,
                Department = QA_M_Department
            };
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 4,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            var userCreateResult = userManager.Create(adminUser, QA_M_Password);
            if (!userCreateResult.Succeeded)
            {
                throw new Exception(string.Join("; ", userCreateResult.Errors));
            }

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            foreach (var roleName in QA_M_Role)
            {
                var roleExist = roleManager.RoleExists(roleName);
                if (!roleExist)
                {
                    var role = new IdentityRole();
                    role.Name = roleName;
                    var roleCreateResult = roleManager.Create(new IdentityRole(roleName));
                    if (!roleCreateResult.Succeeded)
                    {
                        throw new Exception(string.Join("; ", roleCreateResult.Errors));
                    }
                    var addTrainingstaffRoleResult = userManager.AddToRole(adminUser.Id, roleName);
                    if (!addTrainingstaffRoleResult.Succeeded)
                    {
                        throw new Exception(string.Join("; ", addTrainingstaffRoleResult.Errors));
                    }
                }
            }


        }

        private void Create_Staff_HR(ApplicationDbContext context, string QA_M_Email, string QA_M_UserName, string QA_M_Password, string[] QA_M_Role, string QA_M_Department)
        {
            var adminUser = new ApplicationUser
            {
                UserName = QA_M_UserName,
                Email = QA_M_Email,
                Department = QA_M_Department
            };
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 4,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            var userCreateResult = userManager.Create(adminUser, QA_M_Password);
            if (!userCreateResult.Succeeded)
            {
                throw new Exception(string.Join("; ", userCreateResult.Errors));
            }

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            foreach (var roleName in QA_M_Role)
            {
                var roleExist = roleManager.RoleExists(roleName);
                if (!roleExist)
                {
                    var role = new IdentityRole();
                    role.Name = roleName;
                    var roleCreateResult = roleManager.Create(new IdentityRole(roleName));
                    if (!roleCreateResult.Succeeded)
                    {
                        throw new Exception(string.Join("; ", roleCreateResult.Errors));
                    }
                    var addTrainingstaffRoleResult = userManager.AddToRole(adminUser.Id, roleName);
                    if (!addTrainingstaffRoleResult.Succeeded)
                    {
                        throw new Exception(string.Join("; ", addTrainingstaffRoleResult.Errors));
                    }
                }
            }


        }

        private void Create_Staff_IT(ApplicationDbContext context, string QA_M_Email, string QA_M_UserName, string QA_M_Password, string[] QA_M_Role, string QA_M_Department)
        {
            var adminUser = new ApplicationUser
            {
                UserName = QA_M_UserName,
                Email = QA_M_Email,
                Department = QA_M_Department
            };
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 4,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            var userCreateResult = userManager.Create(adminUser, QA_M_Password);
            if (!userCreateResult.Succeeded)
            {
                throw new Exception(string.Join("; ", userCreateResult.Errors));
            }

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            foreach (var roleName in QA_M_Role)
            {
                var roleExist = roleManager.RoleExists(roleName);
                if (!roleExist)
                {
                    var role = new IdentityRole();
                    role.Name = roleName;
                    var roleCreateResult = roleManager.Create(new IdentityRole(roleName));
                    if (!roleCreateResult.Succeeded)
                    {
                        throw new Exception(string.Join("; ", roleCreateResult.Errors));
                    }
                    var addTrainingstaffRoleResult = userManager.AddToRole(adminUser.Id, roleName);
                    if (!addTrainingstaffRoleResult.Succeeded)
                    {
                        throw new Exception(string.Join("; ", addTrainingstaffRoleResult.Errors));
                    }
                }
            }


        }

    }
}

