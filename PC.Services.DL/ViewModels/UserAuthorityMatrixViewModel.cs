using PC.Services.Core.Models;

namespace PC.Services.DL.ViewModels
{
    public class UserAuthorityMatrixViewModel
    {
        public UserAuthorityMatrixViewModel()
        {
            AuthorityMatrix = new List<UserAuthority>();
        }

        public string UserId { get; set; }
        public string Username { get; set; }
        public List<UserAuthority> AuthorityMatrix { get; set; }
    }
}
