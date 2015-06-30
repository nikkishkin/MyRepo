using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManager.Web.Models
{
    public class UserListModel
    {
        public string TeamName { get; set; }
        public IEnumerable<UserModel> Members { get; set; }
        public AddUserModel NewUser { get; set; }
    }
}