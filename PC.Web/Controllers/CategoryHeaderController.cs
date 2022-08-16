using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PC.Services.Core;
using PC.Services.Core.Helper.Consts;
using PC.Services.Core.Models;
using PC.Services.Core.Security;
using PC.Services.DL.DbContext;
using PC.Services.DL.ViewModels;
using System.Security.Claims;

namespace PC.Web.Controllers
{
    //Audit when save add this await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
    [Authorize(Roles = SD.Admin)]
    public class CategoryHeaderController : BaseController
    {
        public CategoryHeaderController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           RoleManager<IdentityRole> roleManager,
           AppDBContext context,
           IConfiguration config,
           IUnitOfWork unitOfWork) : base(userManager, signInManager, roleManager, context, config, unitOfWork)
        {
        }

        #region CategoryHeader
        /**************CategoryHeader Section******************************************/

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string[] includes = { "MainCategory", "Activity", "Details" };
            var allDetails = await _unitOfWork.CategoryHeader.FindAllIncludeAsync(includes);

            //var allDetails = _context.CategoryHeader
            //            .Include(c => c.MainCategory)
            //            .Include(c => c.Activity)
            //            .Include(c => c.Details)
            //            .ToList();

            List<Category> categoryAll = new List<Category>();
            if (allDetails.ToList().Any())
            {

                foreach (var category in allDetails.ToList())
                {
                    Category obj = new Category();
                    obj.categoryHeaders = category;

                    AuthorityMatrixCategoryHeader authorityId = await GetAuthorityMatrix(category);

                    if (authorityId != null)
                    {
                        obj.authority = authorityId;
                    }

                    categoryAll.Add(obj);
                }

            }
            //var query = _context.CategoryHeader
            //            .Include(c => c.MainCategory)
            //            .Include(c => c.Activity)
            //            .Include(c => c.Dteails)
            //            .ToList();



            return View(categoryAll);
        }

        [HttpGet]
        public IActionResult Create()
        {
            GetLookup();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryHeaderViewModel model)
        {
            if (ModelState.IsValid)
            {
                //search if already linked with category header
                var Exist = await SearchExistCategoryHeader(model);
                if (Exist.CategoryHeaderId > 0)
                {
                    TempData["Message"] = 6;

                    GetLookup();
                    return View(model);
                }

                //******End of Exist

                var categoryHeader = await FillCategoryHeaderAsync(model);

                var result = await _unitOfWork.CategoryHeader.AddAsync(categoryHeader);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                //Add AuthorityMatrixCategoryHeader table
                AuthorityMatrixCategoryHeader matrixCategoryHeader = new AuthorityMatrixCategoryHeader();
                matrixCategoryHeader.AuthorityMatrix = _unitOfWork.AuthorityMatrix.GetById(model.AuthorityId);
                matrixCategoryHeader.AuthorityId = matrixCategoryHeader.AuthorityMatrix.AuthorityId;
                matrixCategoryHeader.CategoryHeader = categoryHeader;
                matrixCategoryHeader.CategoryHeader.CategoryHeaderId = categoryHeader.CategoryHeaderId;

                var result2 = await _unitOfWork.AuthorityMatrixCategoryHeader.AddAsync(matrixCategoryHeader);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                //categoryHeader.AuthorityMatrixCategoryHeader.Add(matrixCategoryHeader);

                if (result != null && result2 != null)
                {
                    TempData["Message"] = 1;

                    //model.CategoryHeaderId = result.CategoryHeaderId;
                    GetLookup();
                    return RedirectToAction(nameof(Index));
                }

                TempData["Message"] = 5;
                GetLookup();
                return View(model);
            }

            GetLookup();
            return View(model);


        }

        public async Task<CategoryHeader> FillCategoryHeaderAsync(CategoryHeaderViewModel model)
        {
            var LoggedInuser = await userManager.GetUserAsync(User);


            CategoryHeader categoryHeader = new CategoryHeader();
            categoryHeader.MainCategoryId = model.MainCategoryId;
            categoryHeader.ActivityId = model.ActivityId;
            categoryHeader.DetailsId = model.DetailsId;
            categoryHeader.LevelCount = model.LevelCount;
            categoryHeader.CreatedBy = LoggedInuser;
            categoryHeader.CreatedById = LoggedInuser.Id;
            categoryHeader.CreatedDateTime = DateTime.Now;

            return categoryHeader;
        }

        public async Task<CategoryHeaderViewModel> SearchExistCategoryHeader(CategoryHeaderViewModel model)
        {
            string[] includes = { "AuthorityMatrixCategoryHeader" };
            var queryExist = await _unitOfWork.CategoryHeader.FindAllAsync(criteria: q => q.DetailsId == model.DetailsId, includes);
            if (queryExist.Any())
            {
                model.CategoryHeaderId = queryExist.ToList()[0].CategoryHeaderId;
                model.MainCategoryId = queryExist.ToList()[0].MainCategoryId;
                model.ActivityId = queryExist.ToList()[0].ActivityId;
                model.DetailsId = queryExist.ToList()[0].DetailsId;
                model.LevelCount = queryExist.ToList()[0].LevelCount;

                //var getlevels = await _unitOfWork.Levels.FindAllAsync(criteria: l => l.CategoryHeaderId == model.CategoryHeaderId);
                //if (getlevels.Any())
                //{
                //    foreach (var level in getlevels)
                //    {
                //        model.Levels.Add(level);
                //    }
                //}
            }


            return model;
        }

        public async Task<IActionResult> AddLevel(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            CategoryHeaderViewModel model = new CategoryHeaderViewModel();

            CategoryHeader categoryHeader = await GetCategoryHeader(id);

            if (categoryHeader != null)
            {
                AuthorityMatrixCategoryHeader authorityId = await GetAuthorityMatrix(categoryHeader);
                model.AuthorityDataNames.authorityMatrixName = authorityId.AuthorityMatrix.Name;

                model.AuthorityDataNames.mainCategoryName = categoryHeader.MainCategory.Name;
                model.AuthorityDataNames.activityName = categoryHeader.Activity.Name;
                model.AuthorityDataNames.detailsName = categoryHeader.Details.Name;

                model.CategoryHeaderId = categoryHeader.CategoryHeaderId;
                model.AuthorityId = authorityId.AuthorityMatrix.AuthorityId;
                model.MainCategoryId = categoryHeader.MainCategory.MainCategoryId;
                model.ActivityId = categoryHeader.Activity.ActivityId;
                model.DetailsId = categoryHeader.Details.DetailsId;

                //string[] includes2 = { "CategoryHeader" };
                IEnumerable<Levels> query = await getLevels(id);

                //check if exist level user
                if (query.Count() > 0)
                {
                    foreach (var level in query.ToList())
                    {
                        UserLevels userLevels = new UserLevels();
                        userLevels.Level = level;

                        //get all levels roles first
                        var LevelUserRole = roleManager.Roles.Where(r => r.Id == level.LevelRoleId).ToList();
                        //get user 
                        //var LevelUser = await userManager.FindByIdAsync(level.ApplicationUserId);
                        // GetRolesAsync returns the list of user Roles
                        //var userRoles = await userManager.GetRolesAsync(LevelUser);
                        //fill Level role Id & name
                        if (LevelUserRole.Count() > 0)
                        {
                            userLevels.roleId = LevelUserRole[0].Id;
                            userLevels.roleName = LevelUserRole[0].Name;
                        }

                        model.UserLevels.Add(userLevels);
                    }
                }

                GetLookup();

                return View(model);
            }

            GetLookup();
            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLevel(CategoryHeaderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var categoryHeader = await _unitOfWork.CategoryHeader.GetByIdAsync(model.CategoryHeaderId);
                if (categoryHeader != null)
                {
                    string[] includes = { "CategoryHeader" };
                    var query = await _unitOfWork.Levels.FindAllAsync(criteria: q => q.CategoryHeaderId == model.CategoryHeaderId);
                    if (query.Count() >= categoryHeader.LevelCount)
                    {
                        TempData["Message"] = 11;
                        return RedirectToAction("AddLevel", new { id = model.CategoryHeaderId });
                    }

                    //check if exist
                    var userExist = query.ToList()
                                    .Where(u => u.CategoryHeaderId == model.CategoryHeaderId
                                    && u.ApplicationUserId == model.LevelUser).FirstOrDefault();
                    if (userExist != null)
                    {
                        TempData["Message"] = 6;
                        return RedirectToAction("AddLevel", new { id = model.CategoryHeaderId });
                    }

                    Levels levels = new Levels();

                    //selected user
                    var user = await userManager.FindByIdAsync(model.LevelUser);

                    levels.CategoryHeader = categoryHeader;
                    levels.CategoryHeader.CategoryHeaderId = categoryHeader.CategoryHeaderId;
                    levels.ApplicationUser = user;

                    var LoggedInuser = await userManager.GetUserAsync(User);
                    levels.CreatedBy = LoggedInuser;
                    levels.CreatedById = LoggedInuser.Id;
                    levels.CreatedDateTime = DateTime.Now;
                    levels.LevelRoleId = model.LevelRole;

                    await _unitOfWork.Levels.AddAsync(levels);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                    //add role to user if not exist
                    var LevelUserRole = roleManager.Roles.Where(r => r.Id == model.LevelRole).ToList();
                    if (!(await userManager.IsInRoleAsync(user, LevelUserRole[0].Name)))
                    {
                        await userManager.AddToRoleAsync(user, LevelUserRole[0].Name);

                    }
                    //var userRoles = await userManager.GetRolesAsync(LevelUser);

                    TempData["Message"] = 1;
                    GetLookup();


                    return RedirectToAction("AddLevel", new { id = model.CategoryHeaderId });
                }

                TempData["Message"] = 5;
                GetLookup();
                return View("Create", model);
            }

            TempData["Message"] = 5;
            GetLookup();
            return RedirectToAction("AddLevel", new { id = model.CategoryHeaderId });


        }

        [HttpGet]
        public async Task<IActionResult> ManageUserLevelRole(int Id)
        {
            ViewBag.levelId = Id;

            var userLevel = await _unitOfWork.Levels.GetByIdAsync(Id);

            if (userLevel == null)
            {
                ViewBag.ErrorMessage = $"Role with Role = {Id} cannot be found";
                return View("NotFound");
            }

            var model = new UserLevels();
            model.Level = userLevel;
            model.roleId = model.Level.LevelRoleId;
            GetLookup();
            return PartialView("_ManageUserLevelRole", model);
            //return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserLevelRole(UserLevels model,
                            string LevelId, string OldroleId, string NewroleId)
        {
            if (OldroleId == null || NewroleId == null)
            {
                TempData["Message"] = 1;
                return Json(true);
            }
            //update user level role 
            var slectedLevel = await _unitOfWork.Levels.GetByIdAsync(Convert.ToInt32(LevelId));
            slectedLevel.LevelRoleId = NewroleId;

            var LoggedInuser = await userManager.GetUserAsync(User);
            slectedLevel.UpdatedBy = LoggedInuser;
            slectedLevel.UpdatedById = LoggedInuser.Id;
            slectedLevel.UpdatedDateTime = DateTime.Now;

            //_context.Entry(olddetails).CurrentValues.SetValues(model);
            var updateLevel = _unitOfWork.Levels.Update(slectedLevel);
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

            //add role to user if not exist
            //selected user
            var user = await userManager.FindByIdAsync(slectedLevel.ApplicationUserId);
            var LevelUserRole = roleManager.Roles.Where(r => r.Id == NewroleId).ToList();
            if (!(await userManager.IsInRoleAsync(user, LevelUserRole[0].Name)))
            {
                await userManager.AddToRoleAsync(user, LevelUserRole[0].Name);

            }

            //remove old role from user
            if (OldroleId != NewroleId && OldroleId != null)
            {
                var LevelUserOldRole = roleManager.Roles.Where(r => r.Id == OldroleId).ToList();
                if (await userManager.IsInRoleAsync(user, LevelUserOldRole[0].Name))
                {
                    await userManager.RemoveFromRoleAsync(user, LevelUserOldRole[0].Name);

                }
            }


            TempData["Message"] = 1;
            return Json(true);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddLevel(int detailsId, Details model)
        //{
        //    if (detailsId != model.DetailsId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var LoggedInuser = await userManager.GetUserAsync(User);
        //            model.UpdatedBy = LoggedInuser;
        //            model.UpdatedById = LoggedInuser.Id;
        //            model.UpdatedDateTime = DateTime.Now;

        //            //old details Id
        //            var olddetails = await _unitOfWork.Details.GetByIdAsync(detailsId);

        //            _context.Entry(olddetails).CurrentValues.SetValues(model);
        //            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!activityExists(model.ActivityId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["Activity"] = new SelectList(_unitOfWork.Activity.GetAllAsync().Result.ToList(), "ActivityId", "Name");
        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var details = await _unitOfWork.Details.GetByIdAsync(id);
        //    _unitOfWork.Details.Delete(details);

        //    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

        //    return RedirectToAction(nameof(Index));
        //}
        private bool activityExists(int id)
        {
            return _context.Activity.Any(e => e.ActivityId == id);
        }

        #endregion
        /**************End CategoryHeader Section******************************************/


        #region get lookup 
        private void GetLookup()
        {
            ViewData["AuthorityMatrix"] = new SelectList(_unitOfWork.AuthorityMatrix.GetAllAsync().Result.ToList(), "AuthorityId", "Name");
            ViewData["MainCategory"] = new SelectList(_unitOfWork.MainCategory.GetAllAsync().Result.ToList(), "MainCategoryId", "Name");

            //get list of users and roles to assign R A C I
            var userList = userManager.Users;
            var roleList = roleManager.Roles.Where(r => r.Name != "Admin" && r.Name != "User");

            ViewData["userList"] = new SelectList(userList, "Id", "UserName");
            ViewData["roleList"] = new SelectList(roleList, "Id", "Name");
        }


        [HttpPost]
        public async Task<IActionResult> GetActivity(string selectedMainCategory)
        {
            var mainCategoryId = Convert.ToInt32(selectedMainCategory);

            string[] includes = { "MainCategory" };
            var query = await _unitOfWork.Activity.FindAllAsync(criteria: q => q.MainCategoryId == mainCategoryId, includes);

            //var query = await _unitOfWork.Activity.GetAllAsync();

            //var list = new List<Activity>();
            if (query.ToList().Any())
            {
                return Json(query);
            }

            return Json(false);
        }

        [HttpPost]
        public async Task<IActionResult> GetDetails(string Selected_Activity)
        {
            var selectedActivity = Convert.ToInt32(Selected_Activity);

            string[] includes = { "Activity" };
            var query = await _unitOfWork.Details.FindAllAsync(criteria: q => q.ActivityId == selectedActivity, includes);

            if (query.ToList().Any())
            {
                return Json(query);
            }

            return Json(false);
        }

        private async Task<AuthorityMatrixCategoryHeader> GetAuthorityMatrix(CategoryHeader category)
        {
            string[] includes2 = { "AuthorityMatrix" };
            var authorityId = await _unitOfWork.AuthorityMatrixCategoryHeader
                                .FindAsync(criteria: q => q.CategoryHeaderId == category.CategoryHeaderId, includes2);
            return authorityId;
        }

        private async Task<CategoryHeader> GetCategoryHeader(int id)
        {
            string[] includes = { "MainCategory", "Activity", "Details" };
            var categoryHeader = await _unitOfWork.CategoryHeader.FindAsync(criteria: q => q.CategoryHeaderId == id, includes);
            return categoryHeader;
        }

        private async Task<IEnumerable<Levels>> getLevels(int id)
        {
            return await _unitOfWork.Levels.FindAllAsync(criteria: q => q.CategoryHeaderId == id);
        }
        #endregion





    }
}
