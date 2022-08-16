using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PC.Services.Core;
using PC.Services.Core.Helper.Consts;
using PC.Services.Core.Models;
using PC.Services.Core.Security;
using PC.Services.DL.DbContext;
using PC.Services.DL.ViewModels;
using PC.Web.Models;
using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace PC.Web.Controllers
{
    //Audit when save add this await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
    [Authorize(Roles = SD.Admin)]
    //[AllowAnonymous]
    public class AdminController : BaseController
    {
        public AdminController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           RoleManager<IdentityRole> roleManager,
           AppDBContext context,
           IConfiguration config,
           IUnitOfWork unitOfWork) : base(userManager, signInManager, roleManager, context, config, unitOfWork)
        {
        }

        #region Account Section
        /**************Account Creation Section******************************************/
        //[AllowAnonymous]
        public IActionResult Dashboard()
        {
            var countAccount = userManager.Users.Count();
            var countRoles = roleManager.Roles.Count();

            var viewModel = new DashboardViewModel();

            viewModel.CountAccount = countAccount;
            viewModel.CountRoles = countRoles;

            return View(viewModel);
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        [AllowAnonymous]
        //[Authorize]

        public IActionResult AddNewAccount()
        {
            AccountViewModel model = GetLookup();

            ViewData["JobTitle"] = new SelectList(_unitOfWork.JobTitle.GetAllAsync().Result.ToList(), "JobTitleId", "JobTitleName");

            return View();
        }

        [HttpPost]
        //[Authorize]
        // [AllowAnonymous]
        public async Task<IActionResult> AddNewAccount(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var LoggedInuser = await userManager.GetUserAsync(User);

                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    UserStatus = model.UserStatus,
                    CreatedDateTime = DateTime.Now,
                    CreatedBy = LoggedInuser.UserName
                };

                // Store user data in AspNetUsers database table
                var result = await userManager.CreateAsync(user, model.Password);

                // If user is successfully created, sign-in the user using
                // SignInManager and redirect to index action of HomeController
                if (result.Succeeded)
                {
                    //Add User Job Title
                    UserJobTitle userJobTitle = new UserJobTitle();
                    userJobTitle.Id = user.Id;
                    userJobTitle.JobTitleId = _unitOfWork.JobTitle.GetByIdAsync(model.Job).Result.JobTitleId; //_context.jobTitle.FindAsync(model.Job).Result.JobTitleId;
                    await _context.UserJobTitle.AddAsync(userJobTitle);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                    TempData["Message"] = 1;
                    return RedirectToAction("EditUser", new { Id = user.Id });
                }

                // If there are any errors, add them to the ModelState object
                // which will be displayed by the validation summary tag helper
                foreach (var error in result.Errors)
                {

                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            model = GetLookup();
            ViewData["JobTitle"] = new SelectList(_unitOfWork.JobTitle.GetAllAsync().Result.ToList(), "JobTitleId", "JobTitleName");
            return View(model);
        }

        [AcceptVerbs("Get", "Post")]
        //[Authorize]
        //[AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already in use.");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ChangeUserPassword(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            ChangePasswordViewModel model = new ChangePasswordViewModel
            {
                Id = user.Id,
                username = user.UserName
            };
            return View(model);
        }

        [HttpPost] // don't forget to use [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUserPassword(ChangePasswordViewModel model, string Id)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                // ChangePasswordAsync changes the user password
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var result = await userManager.ResetPasswordAsync(user, token, model.NewPassword);

                // The new password did not meet the complexity rules or
                // the current password is incorrect. Add these errors to
                // the ModelState and rerender ChangePassword view
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                // Upon successfully changing the password refresh sign-in cookie
                //await signInManager.RefreshSignInAsync(user);

                //Log the password values in 
                UserPassword userPassword = new UserPassword();
                userPassword.UserId = user.Id;
                userPassword.PasswordChange = DateTime.Now;
                userPassword.OldPassword = model.CurrentPassword;
                userPassword.NewPassword = model.NewPassword;

                await _context.UserPassword.AddAsync(userPassword);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                //update isFisrt sign in field in db
                user.FirstLogin = false;
                await userManager.UpdateAsync(user);

                TempData["Message"] = 1;
                return RedirectToAction("ListUsers");
            }
            //TempData["Message"] = 10;

            return View(model);
        }

        /**************End of Account Creation Section******************************************/
        #endregion

        #region Role Section
        /**************Role Section******************************************/

        [HttpGet]
        //[Authorize("ListRoles-AdminController")]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        //[AllowAnonymous]
        [HttpGet]
        public IActionResult CreateRole()
        {
            //CreateRoleViewModel model = GetRoles();
            return View();
        }

        //[AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                // We just need to specify a unique role name to create a new role
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                // Saves the role in the underlying AspNetRoles table
                IdentityResult result = await roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    TempData["Message"] = 1;
                    return RedirectToAction("ListRoles");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        private AccountViewModel GetLookup()
        {
            AccountViewModel model = new AccountViewModel();
            //model.Roles = roleManager.Roles.ToList();
            model.JobTitle = _context.jobTitle.ToList();
            return model;
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            // Find the role by Role ID
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            // Retrieve all the Users
            foreach (var user in userManager.Users)
            {
                // If the user is in this role, add the username to
                // Users property of EditRoleViewModel. This model
                // object is then passed to the view for display
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        // This action responds to HttpPost and receives EditRoleViewModel
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;

                // Update the Role using UpdateAsync
                var result = await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    TempData["Message"] = 1;
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = await roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("ListRoles");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();

            foreach (var user in userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return PartialView("_EditUsersInRolePartial", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }


        /**************End Role Section******************************************/
        #endregion

        #region User Section
        /**************User Section******************************************/
        [HttpGet]
        //[Authorize("ListUsers-AdminController")]
        public IActionResult ListUsers()
        {
            //var users = userManager.Users;
            var users = userManager.Users
                .Include(a => a.UserJobTitle)
                .ThenInclude(g => g.JobTitle)
                .ToListAsync().Result;
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            string[] includes = { "AuthorityMatrix" };
            // Get UserAuthorityMatrix retunrs the list of userMatrix
            var UserAuthorityMatrix = _unitOfWork.UserMatrix.FindAllAsync(criteria: q => q.Id == user.Id, includes).Result;

            // GetClaimsAsync retunrs the list of user Claims
            var userClaims = await userManager.GetClaimsAsync(user);

            // GetRolesAsync returns the list of user Roles
            var userRoles = await userManager.GetRolesAsync(user);
            //get job title
            var userJobTitle = await _context.UserJobTitle.Where(u => u.ApplicationUser.Id == user.Id).FirstOrDefaultAsync();
            int? jobId = null;
            if (userJobTitle == null)
            {
                jobId = 0;
            }
            else
            {
                jobId = userJobTitle.JobTitleId;
            }
            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                UserStatus = user.UserStatus,
                Job = jobId,

                Claims = userClaims.Select(c => c.Value).ToList(),
                Roles = userRoles,
                UserAuthorityMatrix = UserAuthorityMatrix.ToList()
            };

            ViewData["JobTitle"] = new SelectList(_unitOfWork.JobTitle.GetAllAsync().Result.ToList(), "JobTitleId", "JobTitleName");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                ViewData["JobTitle"] = new SelectList(_unitOfWork.JobTitle.GetAllAsync().Result.ToList(), "JobTitleId", "JobTitleName");
                return View("NotFound");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;
                user.UserStatus = model.UserStatus;

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    //Add User Job Title
                    if (model.Job != null)
                    {
                        //check if exist just delete and add new one
                        var oldJobTitle = await _context.UserJobTitle.Where(u => u.Id == user.Id).FirstOrDefaultAsync();
                        if (oldJobTitle != null)
                        {
                            _context.UserJobTitle.Remove(oldJobTitle);
                            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                        }

                        UserJobTitle userJobTitle = new UserJobTitle();
                        userJobTitle.ApplicationUser = user;
                        userJobTitle.Id = user.Id;
                        userJobTitle.JobTitle = await _context.jobTitle.FindAsync(model.Job);
                        userJobTitle.JobTitleId = userJobTitle.JobTitle.JobTitleId;

                        await _context.UserJobTitle.AddAsync(userJobTitle);
                        await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                    }


                    TempData["Message"] = 1;
                    return RedirectToAction("EditUser", new { Id = model.Id });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                ViewData["JobTitle"] = new SelectList(_unitOfWork.JobTitle.GetAllAsync().Result.ToList(), "JobTitleId", "JobTitleName");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.UserStatus = false;
                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("ListUsers");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string Id)
        {
            ViewBag.userId = Id;

            var user = await userManager.FindByIdAsync(Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {Id} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRolesViewModel>();

            foreach (var role in roleManager.Roles)
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.IsSelected = true;
                }
                else
                {
                    userRolesViewModel.IsSelected = false;
                }

                model.Add(userRolesViewModel);
            }

            return PartialView("_ManageUserRolesPartial", model);
            //return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var roles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }

            result = await userManager.AddToRolesAsync(user,
                model.Where(x => x.IsSelected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }

            return RedirectToAction("EditUser", "Admin", new { Id = userId });
        }


        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            // UserManager service GetClaimsAsync method gets all the current claims of the user
            var existingUserClaims = await userManager.GetClaimsAsync(user);

            var model = new UserClaimsViewModel
            {
                UserId = userId
            };

            // Loop through each claim we have in our application
            //foreach (Claim claim in ClaimsStore.AllClaims)
            foreach (Claim claim in GetAllControllerActionsUpdated())
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };

                // If the user has the claim, set IsSelected property to true, so the checkbox
                // next to the claim is checked on the UI
                if (existingUserClaims.Any(c => c.Type == claim.Type))
                {
                    userClaim.IsSelected = true;
                }

                model.Cliams.Add(userClaim);
            }

            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.UserId} cannot be found";
                return View("NotFound");
            }

            // Get all the user existing claims and delete them
            var claims = await userManager.GetClaimsAsync(user);
            var result = await userManager.RemoveClaimsAsync(user, claims);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }

            // Add all the claims that are selected on the UI
            result = await userManager.AddClaimsAsync(user,
                model.Cliams.Where(c => c.IsSelected).Select(c => new Claim(c.ClaimType, c.ClaimType)));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected claims to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = model.UserId });

        }

        /**************End User Section******************************************/
        #endregion

        #region Get All Controller Action
        public List<Claim> GetAllControllerActionsUpdated()
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            var controlleractionlist = asm.GetTypes()
                .Where(type => typeof(Controller).IsAssignableFrom(type))
                .SelectMany(type =>
                    type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                .Where(m => !m.GetCustomAttributes(typeof(CompilerGeneratedAttribute),
                    true).Any())
                .Select(x => new
                {
                    Controller = x.DeclaringType.Name,
                    Action = x.Name,
                    ReturnType = x.ReturnType.Name,
                    Attributes = string.Join(",",
                        x.GetCustomAttributes().Select(a => a.GetType().Name.Replace("Attribute", "")))
                })
                .OrderBy(x => x.Controller).ThenBy(x => x.Action).ToList();

            var AllClaims = new List<Claim>();


            foreach (var item in controlleractionlist)
            {
                var ClaimName = item.Action + "-" + item.Controller;

                var claim = new Claim(ClaimName, ClaimName);
                AllClaims.Add(claim);
            }
            return AllClaims;
        }
        #endregion

        #region Application Setting
        //[Authorize("AppSetting-AdminController")]
        public IActionResult AppSetting()
        {
            var model = FillAppSetting();

            return View(model);
        }
        [HttpPost]
        public IActionResult AppSetting(Setting model)
        {
            if (ModelState.IsValid)
            {
                UpdateSetting(model);

                TempData["Message"] = 1;
                return View(model);
            }

            ModelState.AddModelError("", "يجب ادخال جميع الاعدادات");
            return View(model);
        }

        private static void UpdateSetting(Setting model)
        {
            var appSettingsPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "appsettings.json");
            var json = System.IO.File.ReadAllText(appSettingsPath);

            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.Converters.Add(new ExpandoObjectConverter());
            jsonSettings.Converters.Add(new StringEnumConverter());

            dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(json, jsonSettings);

            config.AppSetting.AccessFailedCount = model.AccessFailedCount;
            config.AppSetting.UserPasswordLength = model.UserPasswordLength;
            config.AppSetting.PassRequireDigit = model.PassRequireDigit;
            config.AppSetting.PassRequireLowercase = model.PassRequireLowercase;
            config.AppSetting.AccessFailedCount = model.AccessFailedCount;
            config.AppSetting.PassRequireUppercase = model.PassRequireUppercase;
            config.AppSetting.PassRequireNonAlphanumeric = model.PassRequireNonAlphanumeric;
            config.AppSetting.UserSessionTimeOut = model.UserSessionTimeOut;
            config.AppSetting.MaxLimitToLockUser = model.MaxLimitToLockUser;
            config.AppSetting.UserConfirmPolicyTitle = model.UserConfirmPolicyTitle;
            config.AppSetting.UserConfirmPolicy = model.UserConfirmPolicy;
            config.AppSetting.ApplicationName = model.ApplicationName;
            config.AppSetting.ApplicationLogo = model.ApplicationLogo;
            config.AppSetting.EmpDataApiUrl = model.EmpDataApiUrl;
            config.AppSetting.EnableConfirmPolicy = model.EnableConfirmPolicy;
            config.AppSetting.EnableRightClick = model.EnableRightClick;

            var newJson = JsonConvert.SerializeObject(config, Formatting.Indented, jsonSettings);
            System.IO.File.WriteAllText(appSettingsPath, newJson);
        }

        private Setting FillAppSetting()
        {
            try
            {
                Setting model = new Setting();
                model.AccessFailedCount = config.GetValue<int>("AppSetting:AccessFailedCount");
                model.UserPasswordLength = config.GetValue<int>("AppSetting:UserPasswordLength");
                model.PassRequireDigit = config.GetValue<bool>("AppSetting:PassRequireDigit");
                model.PassRequireLowercase = config.GetValue<bool>("AppSetting:PassRequireLowercase");
                model.PassRequireUppercase = config.GetValue<bool>("AppSetting:PassRequireUppercase");
                model.PassRequireNonAlphanumeric = config.GetValue<bool>("AppSetting:PassRequireNonAlphanumeric");
                model.UserSessionTimeOut = config.GetValue<int>("AppSetting:UserSessionTimeOut");
                model.MaxLimitToLockUser = config.GetValue<int>("AppSetting:MaxLimitToLockUser");
                model.UserConfirmPolicyTitle = config.GetValue<string>("AppSetting:UserConfirmPolicyTitle");
                model.UserConfirmPolicy = config.GetValue<string>("AppSetting:UserConfirmPolicy");
                model.ApplicationName = config.GetValue<string>("AppSetting:ApplicationName");
                model.ApplicationLogo = config.GetValue<string>("AppSetting:ApplicationLogo");
                model.EmpDataApiUrl = config.GetValue<string>("AppSetting:EmpDataApiUrl");
                model.EnableConfirmPolicy = config.GetValue<string>("AppSetting:EnableConfirmPolicy");
                model.EnableRightClick = config.GetValue<string>("AppSetting:EnableRightClick");

                return model;
            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion

        #region User AuthorityMatrix

        [HttpGet]
        public async Task<IActionResult> ManageUserAuthorityMatrix(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            // UserManager service UserMatrix current Authority Matrix of the user
            //var existingUserClaims = await userManager.GetClaimsAsync(user);
            var existingUserMatrix = _unitOfWork.UserMatrix.FindAllAsync(criteria: q => q.Id == userId, null).Result.ToList();

            var model = new UserAuthorityMatrixViewModel
            {
                UserId = userId,
                Username = user.UserName
            };

            // Loop through each UserMatrix for the selected User
            //foreach (Claim claim in ClaimsStore.AllClaims)
            foreach (AuthorityMatrix matrix in await _unitOfWork.AuthorityMatrix.GetAllAsync())
            {
                UserAuthority userAuthority = new UserAuthority
                {
                    AuthorityName = matrix.Name,
                    AuthorityId = matrix.AuthorityId
                };

                // If the user has the matrix, set IsSelected property to true, so the checkbox
                // next to the authorityMatrix is checked on the UI
                if (existingUserMatrix.Count > 0)
                {
                    if (existingUserMatrix.Any(cus => cus.AuthorityId == matrix.AuthorityId))
                    {
                        userAuthority.AuthorityId = matrix.AuthorityId;
                        userAuthority.IsSelected = true;
                    }
                }
                //if (await _unitOfWork.UserMatrix.GetByIdAsync(user.Id) != null)
                //{
                //    userAuthority.AuthorityId = matrix.AuthorityId;
                //    userAuthority.IsSelected = true;
                //}

                model.AuthorityMatrix.Add(userAuthority);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserAuthorityMatrix(UserAuthorityMatrixViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.UserId} cannot be found";
                return View("NotFound");
            }

            // Get all the user existing claims and delete them
            var UserAuthorityMatrix = _unitOfWork.UserMatrix.FindAllAsync(criteria: q => q.Id == user.Id, null).Result.ToList();
            if (UserAuthorityMatrix.Count > 0)
            {
                foreach (var item in UserAuthorityMatrix)
                {
                    _unitOfWork.UserMatrix.Delete(item);
                }
            }

            // Add all the AuthorityMatrix that are selected on the UI
            foreach (var newAuthority in model.AuthorityMatrix)
            {
                if (newAuthority.IsSelected)
                {
                    UserMatrix userMatrix = new UserMatrix
                    {
                        Id = user.Id,
                        ApplicationUser = user,
                        AuthorityId = newAuthority.AuthorityId,
                        AuthorityMatrix = await _unitOfWork.AuthorityMatrix.GetByIdAsync(newAuthority.AuthorityId)
                    };

                    var result = await _unitOfWork.UserMatrix.AddAsync(userMatrix);
                }
            }

            return RedirectToAction("EditUser", new { Id = model.UserId });

        }
        #endregion





    }
}
