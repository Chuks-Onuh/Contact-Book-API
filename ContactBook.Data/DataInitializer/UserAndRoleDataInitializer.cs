using ContactBook.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ContactBook.Data.DataInitializer
{
    public class UserAndRoleDataInitializer
    {
        public static async Task SeedData(ContactBookContext context, UserManager<AppUser> userManager,
               RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();
            await SeedRoles(roleManager);
            await SeedUsers(userManager);
        }
        private static async Task SeedUsers(UserManager<AppUser> userManager)
        {
            if (userManager.FindByEmailAsync("jamesjohn@gmail.com").Result == null)
            {
                AppUser user = new AppUser
                {
                    FirstName = "James",
                    LastName = "John",
                    Email = "jamesjohn@gmail.com",
                    ImageUrl = "openForCorrection",
                    FacebookUrl = "facebookurl",
                    TwitterUrl = "twitterurl",
                    UserName = "jamesjohn@gmail.com",
                    PhoneNumber = "234802570734",
                    City = "Ibadan",
                    State = "Oyo",
                    Country = "Nigeria"
                };
                IdentityResult result = userManager.CreateAsync(user, "Password@1").Result;
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                }
            }
            if (userManager.FindByEmailAsync("chuksonuh@gmail.com").Result == null)
            {
                AppUser user = new AppUser
                {
                    FirstName = "Chuks",
                    LastName = "Onuh",
                    Email = "chuksonuh@gmail.com",
                    UserName = "chuksonuh@gmail.com",
                    ImageUrl = "openForCorrection",
                    FacebookUrl = "facebookurl",
                    TwitterUrl = "twitterurl",
                    PhoneNumber = "+234704536734",
                    City = "Enugu",
                    State = "Enugu",
                    Country = "Nigeria"
                };
                IdentityResult result = userManager.CreateAsync(user, "Password@2").Result;
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            //if (roleManager.RoleExistsAsync("Regular").Result == false)
            //{
            //    var role = new IdentityRole
            //    {
            //        Name = "Regular"
            //    };
            //    await roleManager.CreateAsync(role);
            //}

            if (roleManager.RoleExistsAsync("Admin").Result == false)
            {
                var role = new IdentityRole
                {
                    Name = "Admin"
                };

                await roleManager.CreateAsync(role);
            }
        }
    }      
}
