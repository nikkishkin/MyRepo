using System.Collections.Generic;
using TaskManager.Web.Models.Users;

namespace TaskManager.Web.Models.Teams
{
    public class TeamModel
    {
        public string Name { get; set; }
        public IEnumerable<UserModel> Members { get; set; }
        public int Id { get; set; }
    }
}