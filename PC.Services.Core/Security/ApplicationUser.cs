using Microsoft.AspNetCore.Identity;
using PC.Services.Core.Models;

namespace PC.Services.Core.Security
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public override string UserName { get => base.UserName; set => base.UserName = value; }
        public DateTime CreatedDateTime  { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public bool? FirstLogin { get; set; }
        public bool? MaxMonthLock { get; set; }
        public bool? MonthLockStatus { get; set; }

        public bool? UserStatus { get; set; }

        public IList<UserJobTitle> UserJobTitle { get; set; }
        public IList<UserMatrix> UserMatrix { get; set; }


        //public string Email { get; set; }
    }
}
