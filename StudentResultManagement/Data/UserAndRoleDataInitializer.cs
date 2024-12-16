using Microsoft.AspNetCore.Identity;

namespace StudentResultManagement.Data
{
    public class UserAndRoleDataInitializer
    {
        public static void SeedData(UserManager<IdentityUser> userManager)
        {
            
            SeedUsers(userManager);
        }

        private static void SeedUsers(UserManager<IdentityUser> userManager)
        {
            if (userManager.FindByEmailAsync("johndoe@localhost").Result == null)
            {
                var user = new IdentityUser();
                user.UserName = "johndoe@localhost";
                user.Email = "johndoe@localhost";
               

                IdentityResult result = userManager.CreateAsync(user, "P@ssw0rd1!").Result;

              
            }

            if (userManager.FindByEmailAsync("admin@localhost").Result == null)
            {
                var user = new IdentityUser();
                user.UserName = "admin@localhost";
                user.Email = "admin@localhost";


                IdentityResult result = userManager.CreateAsync(user, "1234Ab@789").Result;


            }



        }

       
    }
}
