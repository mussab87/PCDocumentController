using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PC.Services.Core;
using PC.Services.Core.EmailModel;
using PC.Services.Core.Helper.Consts;
using PC.Services.Core.Helper.Enums;
using PC.Services.Core.Interfaces;
using PC.Services.Core.Models;
using PC.Services.Core.Security;
using PC.Services.DL.DbContext;
using PC.Services.DL.ViewModels;
using PC.Web.Models;
using System.Security.Claims;


namespace PC.Web.Controllers
{
    [Authorize(Roles = SD.User + "," + SD.Admin)]
    public class ApprovalUserController : BaseController
    {
        public ApprovalUserController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           RoleManager<IdentityRole> roleManager,
           AppDBContext context,
           IConfiguration config,
           IUnitOfWork unitOfWork,
           ISendEmail sendEmail) : base(userManager, signInManager, roleManager, context, config, unitOfWork, sendEmail)
        {
        }

        #region ApprovalUser
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
        public async Task<IActionResult> ShowApprovalUser(int catId, string levelRoleName, string userId)
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
                                if (checkRole != null && checkRole.roleName == "A")

                                {
                                    //in case level has only one row 
                                    if (app == 0 && getApprovals.ToList()[app].ApprovalStatusId == (int)ApprovalType.NoAction
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

                                    if (app > 0 && getApprovals.ToList()[app - 1].ApprovalStatusId == (int)ApprovalType.GoForward
                                        && getApprovals.ToList()[app].ApprovalStatusId == (int)ApprovalType.NoAction
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

                                    if (app > 0 && getApprovals.ToList()[app - 1].ApprovalStatusId == (int)ApprovalType.Approved
                                        && getApprovals.ToList()[app].ApprovalStatusId == (int)ApprovalType.NoAction
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
        public async Task<IActionResult> ApproveApprovalUser(string approvalId, string CategoryHeaderId)
        {
            if (approvalId == null)
                return Json(new { message = "Error" });

            var LoggedInuser = await userManager.GetUserAsync(User);

            var Id = Convert.ToInt32(approvalId);
            var catId = Convert.ToInt32(CategoryHeaderId);

            var approvalRow = await _unitOfWork.Approval.GetByIdAsync(Id);
            approvalRow.ApprovalStatusId = (int)ApprovalType.Approved;
            approvalRow.UpdatedBy = LoggedInuser;
            approvalRow.UpdatedById = LoggedInuser.Id;
            approvalRow.UpdatedDateTime = DateTime.Now;

            _unitOfWork.Approval.Update(approvalRow);
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

            #region send Email
            //send email to I if exist in same level
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
            }
            IEnumerable<Levels> query = await getLevels(model.CategoryHeaderId);

            var sendEmail = SendUserIEmail(query, LoggedInuser, model);
            //*********************************************************
            #endregion

            return Json(true);
        }

        [HttpGet]
        public async Task<IActionResult> RejectApprovalUser(string approvalId, string comment, string CategoryHeaderId)
        {
            if (approvalId == null)
                return Json(new { message = "Error" });

            var LoggedInuser = await userManager.GetUserAsync(User);

            var Id = Convert.ToInt32(approvalId);
            var catId = Convert.ToInt32(CategoryHeaderId);

            var approvalRow = await _unitOfWork.Approval.GetByIdAsync(Id);
            approvalRow.UpdatedBy = LoggedInuser;
            approvalRow.UpdatedById = LoggedInuser.Id;
            approvalRow.UpdatedDateTime = DateTime.Now;

            approvalRow.ApprovalStatusId = (int)ApprovalType.Rejected;
            if (comment != null)
            {
                approvalRow.RejectionReason = comment;
            }


            _unitOfWork.Approval.Update(approvalRow);
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

            #region send Email
            //send email to I if exist in same level
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
            }
            IEnumerable<Levels> query = await getLevels(model.CategoryHeaderId);
            var sendEmail = SendUserIEmail(query, LoggedInuser, model);
            //*********************************************************
            #endregion

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

        private bool activityExists(int id)
        {
            return _context.Activity.Any(e => e.ActivityId == id);
        }

        #endregion
        /**************End ApprovalUser Section******************************************/


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

        #region send Email

        public bool SendUserIEmail(IEnumerable<Levels> Level, ApplicationUser LoggedInuser, CreateResponsibleViewModel model)
        {
            try
            {
                bool result = false;
                if (Level.Count() > 0)
                {
                    foreach (var level in Level.ToList())
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
                            if (LevelUserRole[0].Name == "I")
                            {
                                //get user email
                                string userEmail = null;
                                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                                    userEmail = "ahmed.mohamed@procare.com.sa"; //"mussab87@gmail.com";

                                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
                                    userEmail = "ahmed.mohamed@procare.com.sa"; //userManager.FindByIdAsync(userLevels.Level.ApplicationUserId).Result.Email;

                                //get user logged in url
                                var Userurl = HttpContext.Request.GetEncodedUrl();
                                var url = Userurl.Split("/").ToList();
                                var sendUrl = url[0] + "//" + url[2];
                                sendUrl = sendUrl + "/Approval/ShowRequestInformed?catId=" + model.CategoryHeaderId + "&levelRoleName=I&userId=" + userLevels.Level.ApplicationUserId + "&showInformed=I";

                                string messageBody = "<font><h2>New Authority Matrix Action has been added  : </h2></font><br />" +

                                "<br />Click below link to navigate to the request: " +
                                "<br /> " + sendUrl +
                                "<br /><br /> <h1> ProCare Notification Email </h1>";

                                //send email here
                                EmailInfo emailInfo = new EmailInfo();
                                emailInfo.From = config.GetValue<string>("AppSetting:FromEmail");
                                emailInfo.SmtpCredentials = config.GetValue<string>("AppSetting:SmtpCredentials");
                                emailInfo.To = userEmail;
                                emailInfo.Subject = config.GetValue<string>("AppSetting:Subject");
                                emailInfo.SmtpClient = config.GetValue<string>("AppSetting:SmtpClient");
                                emailInfo.SmtpPort = config.GetValue<int>("AppSetting:SmtpPort");
                                emailInfo.UseDefaultCredentials = config.GetValue<bool>("AppSetting:UseDefaultCredentials");
                                emailInfo.EnableSsl = config.GetValue<bool>("AppSetting:EnableSsl");
                                emailInfo.messageBody = messageBody;

                                _sendEmail.SendEmail(emailInfo);
                                result = true;

                            }
                        }
                    }
                }
                return result;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }

        #endregion




    }
}
