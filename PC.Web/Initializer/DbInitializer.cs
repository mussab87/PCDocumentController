using IdentityModel;
using Microsoft.AspNetCore.Identity;
using PC.Services.Core.Helper.Consts;
using PC.Services.Core.Security;
using PC.Services.DL.DbContext;
using System.Security.Claims;

namespace PC.Web.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly AppDBContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(AppDBContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public void Initialize()
        {
            if (_roleManager.FindByNameAsync(SD.Admin).Result == null)
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.User)).GetAwaiter().GetResult();
            }
            else { return; }

            ApplicationUser adminUser = new ApplicationUser()
            {
                UserName = "admin",
                Email = "admin1@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "111111111111",
                FirstName="admin",
                LastName="test"
            };

            _userManager.CreateAsync(adminUser, "Admin@123456").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(adminUser, SD.Admin).GetAwaiter().GetResult();

            var temp1 = _userManager.AddClaimsAsync(adminUser, new Claim[] {
                new Claim(JwtClaimTypes.Name,adminUser.FirstName+" "+ adminUser.LastName),
                new Claim(JwtClaimTypes.GivenName,adminUser.FirstName),
                new Claim(JwtClaimTypes.FamilyName,adminUser.LastName),
                new Claim(JwtClaimTypes.Role,SD.Admin),
            }).Result;

            ApplicationUser customerUser = new ApplicationUser()
            {
                UserName = "user",
                Email = "user@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "111111111111",
                FirstName = "user",
                LastName = "test"
            };

            _userManager.CreateAsync(customerUser, "Admin@123456").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(customerUser, SD.User).GetAwaiter().GetResult();

            //var temp2 = _userManager.AddClaimsAsync(customerUser, new Claim[] {
            //    new Claim(JwtClaimTypes.Name,customerUser.FirstName+" "+ customerUser.LastName),
            //    new Claim(JwtClaimTypes.GivenName,customerUser.FirstName),
            //    new Claim(JwtClaimTypes.FamilyName,customerUser.LastName),
            //    new Claim(JwtClaimTypes.Role,SD.Customer),
            //}).Result;
        }
    }
}
