using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PC.Services.Core;
using PC.Services.Core.Helper.Consts;
using PC.Services.Core.Helper.Enums;
using PC.Services.Core.Models;
using PC.Services.Core.Security;
using PC.Services.DL.DbContext;
using PC.Services.DL.ViewModels;
using PC.Web.Models;
using System.Security.Claims;

namespace PC.Web.Controllers
{
    [Authorize(Roles = SD.User + "," + SD.Admin)]
    public class ApprovalController : BaseController
    {
        public ApprovalController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           RoleManager<IdentityRole> roleManager,
           AppDBContext context,
           IConfiguration config,
           IUnitOfWork unitOfWork) : base(userManager, signInManager, roleManager, context, config, unitOfWork)
        {
        }

        #region Approval
        /**************Approval Section******************************************/
        [HttpGet]
        public async Task<IActionResult> UserDetails(int id)
        {
            var LoggedInuser = await userManager.GetUserAsync(User);

            string[] includes = { "CategoryHeader.AuthorityMatrixCategoryHeader",
                                    "CategoryHeader.MainCategory", "CategoryHeader.Activity",
                                    "CategoryHeader.Details"};
            var userDetails = await _unitOfWork.Levels.FindAllAsync(criteria: l => l.ApplicationUserId == LoggedInuser.Id, includes);

            //filter by authority matix id
            var filterDetails = new List<Levels>();

            if (userDetails.Count() > 0)
            {
                //fill all levels role for the user
                foreach (var user in userDetails)
                {
                    foreach (var item in user.CategoryHeader.AuthorityMatrixCategoryHeader)
                    {
                        if (item.AuthorityId == id)
                        {
                            filterDetails.Add(user);
                        }
                    }
                }

                if (filterDetails.Count > 0)
                    foreach (var item in filterDetails)
                    {
                        var roleId = item.LevelRoleId;
                        item.LevelRoleId = roleManager.Roles.Where(r => r.Id == roleId).FirstOrDefault().Name;
                    }
            }

            ViewBag.authorityMatrix = _unitOfWork.AuthorityMatrix.GetById(id).Name;

            return View(filterDetails);
        }

        [HttpGet]
        public IActionResult Create()
        {
            GetLookup();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateResponsible(int catId, string levelRoleName, string userId)
        {
            CreateResponsibleViewModel model = new CreateResponsibleViewModel();

            CategoryHeader categoryHeader = await GetCategoryHeader(catId);

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

                GetLookup();

                return View(model);
            }

            GetLookup();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateResponsible(CreateResponsibleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var LoggedInuser = await userManager.GetUserAsync(User);

                //save request in TrsDetails table
                TrsDetails details = await SaveTrsDetails(model, LoggedInuser);

                //save approval for all levels in selected categoryHeader detail
                await SaveApprovalLevels(model, LoggedInuser, details);

                //Add attachment file to attachment table
                await SaveAttachment(model, LoggedInuser, details);

                TempData["Message"] = 1;
                return RedirectToAction("CreateResponsible", new { catId = model.CategoryHeaderId, });
            }

            TempData["Message"] = 5;
            await CreateResponsibleGetAction(model);

            GetLookup();
            return View(model);
        }

        private async Task CreateResponsibleGetAction(CreateResponsibleViewModel model)
        {
            CategoryHeader categoryHeader = await GetCategoryHeader(model.CategoryHeaderId);
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
        }

        [HttpGet]
        public async Task<IActionResult> UpdateResponsible(int catId, int TrsDetailsId, int ApprovalId)
        {
            CreateResponsibleViewModel model = new CreateResponsibleViewModel();

            CategoryHeader categoryHeader = await GetCategoryHeader(catId);

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

                //fill trsData for update it
                TrsDetailData trsDetails = new TrsDetailData();
                trsDetails.TrsDetails = await _unitOfWork.TrsDetails.GetByIdAsync(TrsDetailsId);
                trsDetails.TrsDetails.CreatedBy = await userManager.FindByIdAsync(trsDetails.TrsDetails.CreatedById);
                trsDetails.ApprovalCount = categoryHeader.LevelCount;

                //fill approval 
                trsDetails.Approvals = new List<Approval>();
                Approval approval = new Approval();
                approval = await _unitOfWork.Approval.GetByIdAsync(ApprovalId);
                approval.ApplicationUser = await userManager.FindByIdAsync(approval.ApplicationUserId);

                var userjobTitle = _context.UserJobTitle
                                                    .Include(j => j.JobTitle)
                                                    .Where(j => j.Id == approval.ApplicationUserId).FirstOrDefault();

                approval.ApplicationUser.UserJobTitle = new List<UserJobTitle>();
                approval.ApplicationUser.UserJobTitle.Add(userjobTitle);

                approval.ApprovalStatus = await _unitOfWork.ApprovalStatus.GetByIdAsync(approval.ApprovalStatusId);

                trsDetails.Approvals.Add(approval);

                //fill Attachment
                trsDetails.FileAttachment = new List<Attachment>();
                //Attachment attachment = new Attachment();
                var attachment = await _unitOfWork.Attachment.FindAllAsync(criteria: f => f.TrsDetailsId == trsDetails.TrsDetails.TrsDetailsId);
                if (attachment.Count() > 0)
                {
                    foreach (var att in attachment)
                    {
                        trsDetails.FileAttachment.Add(att);
                    }
                }

                model.TrsData.Add(trsDetails);


                GetLookup();

                return View(model);
            }

            GetLookup();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateResponsible(CreateResponsibleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var LoggedInuser = await userManager.GetUserAsync(User);

                //Update request in TrsDetails table
                var oldDetails = await _unitOfWork.TrsDetails.GetByIdAsync(model.TrsData[0].Approvals[0].TrsDetailsId);
                var newTrsDetail = new TrsDetails();
                newTrsDetail = oldDetails;
                newTrsDetail.UpdatedBy = LoggedInuser;
                newTrsDetail.UpdatedById = LoggedInuser.Id;
                newTrsDetail.UpdatedDateTime = DateTime.Now;
                newTrsDetail.ResponsibleComments = model.ResponsibleComments;
                _context.Entry(oldDetails).CurrentValues.SetValues(newTrsDetail);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                //update back forward status to No action as updated from responsible
                var oldApprovalRow = await _unitOfWork.Approval.GetByIdAsync(model.TrsData[0].Approvals[0].ApprovalId);
                var newApprovalRow = new Approval();
                newApprovalRow = oldApprovalRow;
                newApprovalRow.ApprovalStatusId = 5;
                newApprovalRow.ApprovalStatus = await _unitOfWork.ApprovalStatus.GetByIdAsync(5); //NoAction;
                _context.Entry(oldApprovalRow).CurrentValues.SetValues(newApprovalRow);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                //Add attachment file to attachment table
                await UpdateAttachment(model, LoggedInuser, newTrsDetail);

                TempData["Message"] = 1;
                return RedirectToAction("ShowRequests", new { catId = model.CategoryHeaderId, });
            }
            TempData["Message"] = 5;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ShowRequests(int catId, string levelRoleName, string userId)
        {
            CreateResponsibleViewModel model = new CreateResponsibleViewModel();

            CategoryHeader categoryHeader = await GetCategoryHeader(catId);

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

                //get TrsDetail & approval & attachment for every trsDetail
                var getTrs = await _unitOfWork.TrsDetails.FindAllAsync(criteria: t => t.CategoryHeaderId == categoryHeader.CategoryHeaderId && t.CreatedById == userId);
                if (getTrs.ToList().Count > 0)
                {
                    foreach (var trs in getTrs.ToList())
                    {
                        TrsDetailData trsDetailData = new TrsDetailData();
                        trsDetailData.TrsDetails = trs;

                        //get list of approval level users
                        string[] include = { "ApplicationUser.UserJobTitle" };
                        var getApprovals = await _unitOfWork.Approval.FindAllAsync(criteria: t => t.TrsDetailsId == trs.TrsDetailsId, include);

                        if (getApprovals.ToList().Count > 0)
                        {
                            trsDetailData.ApprovalCount = getApprovals.ToList().Count;

                            trsDetailData.Approvals = new List<Approval>();
                            foreach (var app in getApprovals.ToList())
                            {
                                app.ApplicationUser = await userManager.FindByIdAsync(app.ApplicationUserId);
                                var userjobTitle = _context.UserJobTitle
                                                    .Include(j => j.JobTitle)
                                                    .Include(j => j.ApplicationUser)
                                                    .Where(j => j.Id == app.ApplicationUserId).FirstOrDefault();

                                userjobTitle.ApplicationUser = await userManager.FindByIdAsync(userjobTitle.Id);
                                //userjobTitle.ApplicationUser.jo = await _unitOfWork.JobTitle.GetByIdAsync(userjobTitle.JobTitleId);
                                app.ApplicationUser.UserJobTitle = new List<UserJobTitle>();
                                app.ApplicationUser.UserJobTitle.Add(userjobTitle);

                                app.ApprovalStatus = await _unitOfWork.ApprovalStatus.GetByIdAsync(app.ApprovalStatusId);

                                trsDetailData.Approvals.Add(app);
                            }
                        }

                        //get list of attachment for every trsDetail if exist
                        var getAttach = await _unitOfWork.Attachment.FindAllAsync(criteria: t => t.TrsDetailsId == trs.TrsDetailsId);
                        if (getAttach.ToList().Count > 0)
                        {
                            trsDetailData.FileAttachment = new List<Attachment>();
                            foreach (var att in getAttach.ToList())
                            {
                                trsDetailData.FileAttachment.Add(att);
                            }
                        }

                        model.TrsData.Add(trsDetailData);
                    }
                }

                GetLookup();

                return View(model);
            }

            GetLookup();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ShowRequestInformed(int catId, string levelRoleName, string userId)
        {
            CreateResponsibleViewModel model = new CreateResponsibleViewModel();

            CategoryHeader categoryHeader = await GetCategoryHeader(catId);

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

                //get user level for evry user in every row level for getting pemessions
                IEnumerable<Levels> query = await getLevels(categoryHeader.CategoryHeaderId);
                if (query.Count() > 0)
                {
                    foreach (var level in query.ToList())
                    {
                        UserLevels userLevels = new UserLevels();
                        userLevels.Level = level;

                        var LevelUserRole = roleManager.Roles.Where(r => r.Id == level.LevelRoleId).ToList();
                        if (LevelUserRole.Count() > 0)
                        {
                            userLevels.roleId = LevelUserRole[0].Id;
                            userLevels.roleName = LevelUserRole[0].Name;
                        }
                        model.UserLevels.Add(userLevels);
                    }
                }

                //get TrsDetail & approval & attachment for every trsDetail
                var getTrs = await _unitOfWork.TrsDetails.FindAllAsync(criteria: t => t.CategoryHeaderId == categoryHeader.CategoryHeaderId);
                if (getTrs.ToList().Count > 0)
                {
                    foreach (var trs in getTrs.ToList())
                    {
                        TrsDetailData trsDetailData = new TrsDetailData();
                        trs.CreatedBy = await userManager.FindByIdAsync(trs.CreatedById);
                        trsDetailData.TrsDetails = trs;

                        //get list of approval level users
                        string[] include = { "ApplicationUser.UserJobTitle" };
                        var getApprovals = await _unitOfWork.Approval.FindAllAsync(criteria: t => t.TrsDetailsId == trs.TrsDetailsId, include);

                        if (getApprovals.ToList().Count > 0)
                        {
                            trsDetailData.ApprovalCount = getApprovals.ToList().Count;

                            trsDetailData.Approvals = new List<Approval>();
                            foreach (var app in getApprovals.ToList())
                            {
                                app.ApplicationUser = await userManager.FindByIdAsync(app.ApplicationUserId);
                                var userjobTitle = _context.UserJobTitle
                                                    .Include(j => j.JobTitle)
                                                    .Include(j => j.ApplicationUser)
                                                    .Where(j => j.Id == app.ApplicationUserId).FirstOrDefault();

                                userjobTitle.ApplicationUser = await userManager.FindByIdAsync(userjobTitle.Id);
                                //userjobTitle.ApplicationUser.jo = await _unitOfWork.JobTitle.GetByIdAsync(userjobTitle.JobTitleId);
                                app.ApplicationUser.UserJobTitle = new List<UserJobTitle>();
                                app.ApplicationUser.UserJobTitle.Add(userjobTitle);

                                app.ApprovalStatus = await _unitOfWork.ApprovalStatus.GetByIdAsync(app.ApprovalStatusId);

                                trsDetailData.Approvals.Add(app);
                            }
                        }

                        //get list of attachment for every trsDetail if exist
                        var getAttach = await _unitOfWork.Attachment.FindAllAsync(criteria: t => t.TrsDetailsId == trs.TrsDetailsId);
                        if (getAttach.ToList().Count > 0)
                        {
                            trsDetailData.FileAttachment = new List<Attachment>();
                            foreach (var att in getAttach.ToList())
                            {
                                trsDetailData.FileAttachment.Add(att);
                            }
                        }

                        model.TrsData.Add(trsDetailData);
                    }
                }

                GetLookup();

                return View(model);
            }

            GetLookup();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ShowConsulted(int catId, string levelRoleName, string userId)
        {
            CreateResponsibleViewModel model = new CreateResponsibleViewModel();

            CategoryHeader categoryHeader = await GetCategoryHeader(catId);

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

                //get user level for evry user in every row level for getting pemessions
                IEnumerable<Levels> query = await getLevels(categoryHeader.CategoryHeaderId);
                if (query.Count() > 0)
                {
                    foreach (var level in query.ToList())
                    {
                        UserLevels userLevels = new UserLevels();
                        userLevels.Level = level;

                        var LevelUserRole = roleManager.Roles.Where(r => r.Id == level.LevelRoleId).ToList();
                        if (LevelUserRole.Count() > 0)
                        {
                            userLevels.roleId = LevelUserRole[0].Id;
                            userLevels.roleName = LevelUserRole[0].Name;
                        }
                        model.UserLevels.Add(userLevels);
                    }
                }

                //get TrsDetail & approval & attachment for every trsDetail
                var getTrs = await _unitOfWork.TrsDetails.FindAllAsync(criteria: t => t.CategoryHeaderId == categoryHeader.CategoryHeaderId);
                if (getTrs.ToList().Count > 0)
                {
                    foreach (var trs in getTrs.ToList())
                    {
                        TrsDetailData trsDetailData = new TrsDetailData();
                        trsDetailData.TrsDetails = trs;
                        trsDetailData.TrsDetails.CreatedBy = await userManager.FindByIdAsync(trs.CreatedById);

                        //get list of approval level users
                        string[] include = { "ApplicationUser.UserJobTitle" };
                        var getApprovals = await _unitOfWork.Approval.FindAllAsync(criteria: t => t.TrsDetailsId == trs.TrsDetailsId, include);
                        var LoggedInuser = await userManager.GetUserAsync(User);
                        if (getApprovals.ToList().Count > 0)
                        {
                            trsDetailData.ApprovalCount = getApprovals.ToList().Count;

                            trsDetailData.Approvals = new List<Approval>();
                            for (int app = 0; app < getApprovals.ToList().Count; app++)
                            {
                                getApprovals.ToList()[app].ApplicationUser = await userManager.FindByIdAsync(getApprovals.ToList()[app].ApplicationUserId);
                                var userjobTitle = _context.UserJobTitle
                                                    .Include(j => j.JobTitle)
                                                    .Where(j => j.Id == getApprovals.ToList()[app].ApplicationUserId).FirstOrDefault();

                                getApprovals.ToList()[app].ApplicationUser.UserJobTitle.Add(userjobTitle);

                                getApprovals.ToList()[app].ApprovalStatus = await _unitOfWork.ApprovalStatus.GetByIdAsync(getApprovals.ToList()[app].ApprovalStatusId);

                                //check if the user have Consulted role
                                var checkRole = model.UserLevels.Where(u => u.Level.ApplicationUserId == getApprovals.ToList()[app].ApplicationUserId).FirstOrDefault();

                                //var checkRoleForPrevios = model.UserLevels.Where(u => u.Level.ApplicationUserId == getApprovals.ToList()[app - 1].ApplicationUserId).FirstOrDefault();
                                if (checkRole != null && checkRole.roleName == "C")
                                {
                                    if (app == 0
                                    && getApprovals.ToList()[app].ApprovalStatusId != (int)ApprovalType.GoForward
                                    && getApprovals.ToList()[app].ApprovalStatusId != (int)ApprovalType.Rejected
                                    && getApprovals.ToList()[app].ApprovalStatusId != (int)ApprovalType.BackForward
                                    && getApprovals.ToList()[app].ApprovalStatusId != (int)ApprovalType.Approved
                                    && getApprovals.ToList()[app].ApplicationUser.Id == LoggedInuser.Id)
                                    {
                                        trsDetailData.Approvals.Add(getApprovals.ToList()[app]);

                                        //get list of attachment for every trsDetail if exist
                                        var getAttach = await _unitOfWork.Attachment.FindAllAsync(criteria: t => t.TrsDetailsId == trs.TrsDetailsId);
                                        if (getAttach.ToList().Count > 0)
                                        {
                                            trsDetailData.FileAttachment = new List<Attachment>();
                                            foreach (var att in getAttach.ToList())
                                            {
                                                trsDetailData.FileAttachment.Add(att);
                                            }
                                        }

                                        model.TrsData.Add(trsDetailData);
                                    }

                                    if (app > 0
                                    && getApprovals.ToList()[app].ApprovalStatusId != (int)ApprovalType.GoForward
                                    && getApprovals.ToList()[app].ApprovalStatusId != (int)ApprovalType.Rejected
                                    && getApprovals.ToList()[app].ApprovalStatusId != (int)ApprovalType.BackForward
                                    && getApprovals.ToList()[app].ApprovalStatusId != (int)ApprovalType.Approved
                                    && getApprovals.ToList()[app - 1].ApprovalStatusId == (int)ApprovalType.GoForward
                                    && getApprovals.ToList()[app - 1].ApprovalStatusId != (int)ApprovalType.BackForward
                                    && getApprovals.ToList()[app - 1].ApprovalStatusId != (int)ApprovalType.NoAction
                                    && getApprovals.ToList()[app].ApplicationUser.Id == LoggedInuser.Id)
                                    {
                                        trsDetailData.Approvals.Add(getApprovals.ToList()[app]);

                                        //get list of attachment for every trsDetail if exist
                                        var getAttach = await _unitOfWork.Attachment.FindAllAsync(criteria: t => t.TrsDetailsId == trs.TrsDetailsId);
                                        if (getAttach.ToList().Count > 0)
                                        {
                                            trsDetailData.FileAttachment = new List<Attachment>();
                                            foreach (var att in getAttach.ToList())
                                            {
                                                trsDetailData.FileAttachment.Add(att);
                                            }
                                        }

                                        model.TrsData.Add(trsDetailData);
                                    }

                                }

                            }
                        }


                    }
                }

                GetLookup();

                return View(model);
            }

            GetLookup();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ApproveConsulted(string approvalId)
        {
            if (approvalId == null)
                return Json(new { message = "Error" });

            var LoggedInuser = await userManager.GetUserAsync(User);

            var Id = Convert.ToInt32(approvalId);
            var approvalRow = await _unitOfWork.Approval.GetByIdAsync(Id);
            approvalRow.ApprovalStatusId = 3;
            approvalRow.UpdatedBy = LoggedInuser;
            approvalRow.UpdatedById = LoggedInuser.Id;
            approvalRow.UpdatedDateTime = DateTime.Now;

            _unitOfWork.Approval.Update(approvalRow);
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

            return Json(true);
        }

        [HttpGet]
        public async Task<IActionResult> BackConsulted(string approvalId, string comment)
        {
            if (approvalId == null)
                return Json(new { message = "Error" });

            var LoggedInuser = await userManager.GetUserAsync(User);

            var Id = Convert.ToInt32(approvalId);
            var approvalRow = await _unitOfWork.Approval.GetByIdAsync(Id);
            approvalRow.UpdatedBy = LoggedInuser;
            approvalRow.UpdatedById = LoggedInuser.Id;
            approvalRow.UpdatedDateTime = DateTime.Now;

            approvalRow.ApprovalStatusId = 4;
            if (comment != null)
            {
                approvalRow.ConsultedComments = comment;
            }


            _unitOfWork.Approval.Update(approvalRow);
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

            return Json(true);
        }

        private async Task<TrsDetails> SaveTrsDetails(CreateResponsibleViewModel model, ApplicationUser LoggedInuser)
        {
            TrsDetails details = new TrsDetails();
            details.CategoryHeaderId = model.CategoryHeaderId;
            details.ResponsibleComments = model.ResponsibleComments;
            details.CreatedBy = LoggedInuser;
            details.CreatedById = LoggedInuser.Id;
            details.CreatedDateTime = DateTime.Now;

            var result = await _unitOfWork.TrsDetails.AddAsync(details);
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
            return details;
        }

        private async Task SaveApprovalLevels(CreateResponsibleViewModel model, ApplicationUser LoggedInuser, TrsDetails details)
        {
            //get levels for adding workflow in Approval table by sequence 
            IEnumerable<Levels> query = await getLevels(model.CategoryHeaderId);

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
                        if (LevelUserRole[0].Name == "C" || LevelUserRole[0].Name == "A")
                        {
                            Approval approval = new Approval();
                            approval.TrsDetailsId = details.TrsDetailsId;
                            approval.TrsDetails = details;
                            approval.ApplicationUserId = level.ApplicationUserId;
                            approval.ApprovalStatusId = 5; //NoAction
                            approval.ApprovalStatus = await _unitOfWork.ApprovalStatus.GetByIdAsync(5); //NoAction

                            approval.CreatedBy = LoggedInuser;
                            approval.CreatedById = LoggedInuser.Id;
                            approval.CreatedDateTime = DateTime.Now;

                            await _unitOfWork.Approval.AddAsync(approval);
                            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                        }
                    }
                }
            }
        }

        private async Task SaveAttachment(CreateResponsibleViewModel model, ApplicationUser LoggedInuser, TrsDetails details)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/RequestAttachments");

            //create folder if not exist
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //get file extension
            FileInfo fileInfo = new FileInfo(model.AttachmentFile.File.FileName);
            var UniqueFileName = string.Format(@"{0}", Guid.NewGuid());
            string fileName = model.AttachmentFile.FileName + UniqueFileName + fileInfo.Extension;

            string fileNameWithPath = Path.Combine(path, fileName);

            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                model.AttachmentFile.File.CopyTo(stream);
            }

            Attachment attachment = new Attachment();
            attachment.TrsDetailsId = details.TrsDetailsId;
            attachment.AttachmentType = fileInfo.Extension;
            attachment.AttachmentPath = fileName;
            attachment.CreatedBy = LoggedInuser;
            attachment.CreatedById = LoggedInuser.Id;
            attachment.CreatedDateTime = DateTime.Now;

            await _unitOfWork.Attachment.AddAsync(attachment);
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        private async Task UpdateAttachment(CreateResponsibleViewModel model, ApplicationUser LoggedInuser, TrsDetails details)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/RequestAttachments");

            //create folder if not exist
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //get file extension
            FileInfo fileInfo = new FileInfo(model.AttachmentFile.File.FileName);
            var UniqueFileName = string.Format(@"{0}", Guid.NewGuid());
            string fileName = model.AttachmentFile.FileName + UniqueFileName + fileInfo.Extension;

            string fileNameWithPath = Path.Combine(path, fileName);

            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                model.AttachmentFile.File.CopyTo(stream);
            }

            Attachment attachment = new Attachment();
            attachment.TrsDetailsId = details.TrsDetailsId;
            attachment.AttachmentType = fileInfo.Extension;
            attachment.AttachmentPath = fileName;
            attachment.UpdatedBy = LoggedInuser;
            attachment.UpdatedById = LoggedInuser.Id;
            attachment.UpdatedDateTime = DateTime.Now;

            await _unitOfWork.Attachment.AddAsync(attachment);
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
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
                //if (query.Count() >= categoryHeader.LevelCount)
                //{
                //    TempData["Message"] = 5;
                //    GetLookup();
                //    return View("Create", model);
                //}

                //Levels levels = new Levels();

                //levels.CategoryHeader = categoryHeader;
                //levels.CategoryHeader.CategoryHeaderId = categoryHeader.CategoryHeaderId;
                //levels.ApplicationUser = await userManager.FindByIdAsync(model.LevelUser);

                //var LoggedInuser = await userManager.GetUserAsync(User);
                //levels.CreatedBy = LoggedInuser;
                //levels.CreatedById = LoggedInuser.Id;
                //levels.CreatedDateTime = DateTime.Now;

                //await _unitOfWork.Levels.AddAsync(levels);
                //await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                //TempData["Message"] = 1;
                //GetLookup();

                //await SearchExistCategoryHeader(model);

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
                        GetLookup();

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

                    //await SearchExistCategoryHeader(model);

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

        private bool activityExists(int id)
        {
            return _context.Activity.Any(e => e.ActivityId == id);
        }

        #endregion
        /**************End Approval Section******************************************/


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
