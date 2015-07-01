using System.Collections.Generic;

namespace TaskManager.Web.Models.Users
{
    public class UserListModel
    {
        public string TeamName { get; set; }
        public IEnumerable<UserModel> Members { get; set; }
        public AddUserModel NewUser { get; set; }
        public int TeamId { get; set; }
    }
}